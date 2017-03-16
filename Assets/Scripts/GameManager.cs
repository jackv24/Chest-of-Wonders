using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Events
    public delegate void NormalEvent();
    public delegate void BoolEvent(bool value);
    public NormalEvent OnLevelLoaded;
    public NormalEvent OnGameOver;
    public BoolEvent OnPausedChange;

    //Static instance for easy access
    public static GameManager instance;

    public int firstSceneIndex = 2;
    private int loadedLevelIndex = -1;

    [Space()]
    public GameObject player;

    [Space()]
    public float playerRespawnDelay = 3f;

    //Game running and game pause are two seperate bools to keep track of in dialogue or menu, or game paused...
    [HideInInspector] public bool gameRunning = true;
    [HideInInspector] public bool gamePaused = false;
    //Public property allows movement, etc, if both conditions are fulfilled
    public bool CanDoActions { get { return gameRunning && !gamePaused; } }

    private PlayerActions playerActions;

    private void Awake()
    {
        //There should only be one game manager present in the scene
        if (!instance)
            instance = this;
        else
        {
            Debug.LogWarning("More than one Game Manager was found in the scene, and has been removed.");

            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerActions = new PlayerActions();

        if (!player)
            player = GameObject.FindWithTag("Player");

        if (player)
        {
            //Register player death as game over
            CharacterStats stats = player.GetComponent<CharacterStats>();
            stats.OnDeath += GameOver;
        }

        //Load the first level
        if (SceneManager.sceneCount <= 1)
            LoadLevel(firstSceneIndex, player ? player.transform.position : Vector3.zero);
        //If level is already open in the editor, use that instead
        else if (SceneManager.sceneCount == 2)
        {
            loadedLevelIndex = SceneManager.GetSceneAt(1).buildIndex;
        }
        else
            Debug.LogWarning("Too many scenes open!");
    }

    private void Update()
    {
        //Toggle pause menu on button press
        if (playerActions.Pause.WasPressed)
            TogglePaused();
    }

    public void LoadLevel(int buildIndex, Vector2 playerPos)
    {
        //Disable and set player position
        player.SetActive(false);
        player.transform.position = playerPos;

        //Start the unload of old level and load of new level
        StartCoroutine("LoadLevelAsync", buildIndex);
    }

    IEnumerator LoadLevelAsync(int buildIndex)
    {
        //If a level is already loaded, unload it
        if (loadedLevelIndex >= 0)
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync(loadedLevelIndex);

            //Wait until level has finished unloading
            yield return async;
        }

        //Load new level additively, and keep track of it as loaded
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
        loadedLevelIndex = buildIndex;

        //Re-enable the player after level is loaded
        player.SetActive(true);

        //Call level loaded events
        if (OnLevelLoaded != null)
            OnLevelLoaded();
    }

    public void GameOver()
    {
        //Call game over events
        if (OnGameOver != null)
            OnGameOver();
    }

    public void TogglePaused()
    {
        //Toggle pause bool
        gamePaused = !gamePaused;

        //Timescale is 0 if game paused, 1 if game not paused
        Time.timeScale = gamePaused ? 0 : 1;

        //Call pause state change events
        if (OnPausedChange != null)
            OnPausedChange(gamePaused);
    }

    private void OnDisable()
    {
        //Reset timescale as scene may be exited when paused
        Time.timeScale = 1;
    }
}
