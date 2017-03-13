using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Events
    public delegate void NormalEvent();
    public delegate void BoolEvent(bool value);
    public NormalEvent OnGameOver;
    public BoolEvent OnPausedChange;

    //Static instance for easy access
    public static GameManager instance;

    public Transform playerSpawn;
    public GameObject playerPrefab;

    private GameObject player;

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

        //Can only spawn player if there is a place to spawn them
        if (playerSpawn)
        {
            //Attempt to find and already existing player first
            player = GameObject.FindWithTag("Player");

            //If no player already exists, then spawn one
            if (!player)
            {
                if (playerPrefab)
                {
                    player = Instantiate(playerPrefab);

                    player.name = playerPrefab.name;
                }
                else
                    Debug.LogWarning("No player prefab assigned to Game Manager");
            }

            if(player)
            {
                CharacterStats stats = player.GetComponent<CharacterStats>();
                stats.OnDeath += GameOver;
            }
        }
        else
            Debug.LogWarning("No player spawn assigned to Game Manager");
    }

    private void Update()
    {
        //Toggle pause menu on button press
        if (playerActions.Pause.WasPressed)
            TogglePaused();
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
}
