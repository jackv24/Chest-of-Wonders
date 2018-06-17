using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
	Playing,
	Paused,
	ExitingLevel,
	EnteringLevel,
	Cutscene,
	GameOver
}

public class GameManager : MonoBehaviour
{
	// Events
	public delegate void LevelLoadedEvent();
    public event LevelLoadedEvent OnLevelLoaded;

	public delegate void GameOverEvent();
    public event GameOverEvent OnGameOver;

    public delegate void PauseChangeEvent(bool value);
    public event PauseChangeEvent OnPausedChange;

	public delegate void GameStateChangeEvent(GameStates fromState, GameStates toState);
	public event GameStateChangeEvent OnGameStateChanged;

	// Game state
	private GameStates gameState;

	public GameStates GameState
	{
		get
		{
			return gameState;
		}
		set
		{
			GameStates oldState = gameState;
			gameState = value;

			OnGameStateChanged?.Invoke(oldState, gameState);
		}
	}

    // Static instance for easy access
    public static GameManager instance;

    private string firstSceneName;
	private string loadedSceneName;

	public string LoadedSceneName { get { return loadedSceneName; } }

    [Space()]
    public GameObject player;

    [Space()]
    public float playerRespawnDelay = 3f;

    [Space()]
    public float levelTransitionTime = 0.25f;

	[Space()]
	public float gamePauseLockDelay = 0.25f;
	private float gamePauseUnlockTime = 0;

    // Public property allows movement, etc, if both conditions are fulfilled
    public bool CanDoActions { get { return GameState == GameStates.Playing; } }

	private SaveData.Location npcSaveLocation;
	private SaveData.Location autoSaveLocation;
	private SaveData.Location lastSaveLocation;

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
		//Handle save/load for self
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				npcSaveLocation = data.npcSave;
				autoSaveLocation = data.autoSave;
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				data.autoSave = lastSaveLocation;

				if (hardSave)
					data.npcSave = lastSaveLocation;
			};
		}

        playerActions = ControlManager.GetPlayerActions();

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
        {
			StartCoroutine(SpawnPlayerStart());
        }
        //If level is already open in the editor, use that instead
        else if (SceneManager.sceneCount == 2)
        {
            loadedSceneName = SceneManager.GetSceneAt(1).name;
        }
        else
            Debug.LogWarning("Too many scenes open!");

        if (!Application.isEditor) Cursor.visible = false;
    }

    private void Update()
    {
        //Toggle pause menu on button press
        if (playerActions.Pause.WasPressed)
            TogglePaused();

        if (!Application.isEditor)
        {
            if (playerActions.Move.WasPressed)
                Cursor.visible = false;
            else if (!CanDoActions)
            {
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                    Cursor.visible = true;
            }
        }
    }

	public void LoadLevel(string sceneName, string doorName)
	{
		//Start the unload of old level and load of new level
		StartCoroutine(ChangeLevel(sceneName, doorName));
	}

    IEnumerator ChangeLevel(string sceneName, string doorName)
    {
		CharacterMove move = player.GetComponent<CharacterMove>();
		PlayerInput input = player.GetComponent<PlayerInput>();

		// Disable input and player will keep running through door
		if (input)
			input.enabled = false;

		if (move)
		{
			move.ignoreCanMove = true;
			move.Move(move.FacingDirection);
		}

		GameState = GameStates.ExitingLevel;

		float fadeTime = levelTransitionTime / 2;

        //If a level is already loaded, unload it
        if (!string.IsNullOrEmpty(loadedSceneName))
        {
            //Fade out
            UIFunctions.instance.ShowLoadingScreen(true, fadeTime);
            yield return new WaitForSeconds(fadeTime);

            AsyncOperation a = SceneManager.UnloadSceneAsync(loadedSceneName);

            //Wait until level has finished unloading
            yield return a;
        }

        //Load new level additively, and keep track of it as loaded
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadedSceneName = sceneName;

        yield return async;

        player.SetActive(false);

        float targetPos = 0;
        bool exitRight = false;

        //If player has come through doorway, place them in correct position
        if (!string.IsNullOrEmpty(doorName))
        {
			SpawnMarker marker = FindSpawnMarker(doorName);

			if (marker)
			{
				Doorway door = marker as Doorway;

				if (door)
				{
					player.transform.position = (Vector2)marker.transform.position;
					targetPos = marker.transform.position.x + door.exitOffset;

					exitRight = targetPos > marker.transform.position.x;
				}
				else
					Debug.LogError($"Spawn Marker: {marker.gameObject.name} is not a doorway!");
			}
		}

		//Return all pooled objects to their pools (prevents things like projectiles persisting between levels)
		ObjectPooler.ReturnAll();

		//Re-enable the player after level is loaded
		player.SetActive(true);

        //Call level loaded events
        if (OnLevelLoaded != null)
            OnLevelLoaded();

		//Save player position outside of door
		lastSaveLocation = new SaveData.Location(loadedSceneName, doorName);

		//Save data during fade out
		SaveManager.instance?.SaveGame(false);

		//Fade in
		UIFunctions.instance.ShowLoadingScreen(false, fadeTime);

        if (move)
            move.MovementState = CharacterMovementStates.Normal;

		GameState = GameStates.EnteringLevel;

        if (input && move && !string.IsNullOrEmpty(doorName))
        {
			// Animate player running out
			player.GetComponent<CharacterAnimator>()?.SetAnimatorAxis(new Vector2(exitRight ? 1 : -1, 0));

            //Move player to doorway exit position
            while ((exitRight && player.transform.position.x < targetPos) || (!exitRight && player.transform.position.x > targetPos))
            {
                move.Move(exitRight ? 1 : -1f);

                yield return new WaitForEndOfFrame();
            }

			move.ignoreCanMove = false;

            input.enabled = true;
        }

		GameState = GameStates.Playing;
    }

    public void GameOver()
    {
		GameState = GameStates.GameOver;

        //Call game over events
        if (OnGameOver != null)
            OnGameOver();
    }

    public void TogglePaused()
    {
		// Toggle game state between paused and playing
		if (GameState == GameStates.Playing)
		{
			GameState = GameStates.Paused;
		}
		else if(GameState == GameStates.Paused)
		{
			GameState = GameStates.Playing;
		}
		else
		{
			// Can only toggle between paused and playing
			return;
		}

		//Only allow pausing again after a delay - prevent mashing pause and causing issues
		if (Time.unscaledTime < gamePauseUnlockTime)
			return;
		gamePauseUnlockTime = Time.unscaledTime + gamePauseLockDelay;

        bool gamePaused = GameState == GameStates.Paused;

        //Timescale is 0 if game paused, 1 if game not paused
        Time.timeScale = gamePaused ? 0 : 1;

        if (!Application.isEditor)
            Cursor.visible = gamePaused;

        //Call pause state change events
        if (OnPausedChange != null)
            OnPausedChange(gamePaused);
    }

    private void OnDisable()
    {
        //Reset timescale as scene may be exited when paused
        Time.timeScale = 1;
    }

	IEnumerator SpawnPlayerStart()
	{
		//Delay for one frame to allow for save/load events to be subscribed
		yield return null;

		SpawnPlayer(false);
	}

    public void SpawnPlayer(bool reset)
    {
        player.SetActive(true);
		GameState = GameStates.Playing;

		SaveManager.instance?.LoadGame(reset);

		SaveData.Location location = reset ? npcSaveLocation : autoSaveLocation;

		//Set first level to be loaded
		firstSceneName = location.sceneName;

		SpawnMarker spawnMarker = FindSpawnMarker(location.spawnMarkerName);
		if (spawnMarker)
		{
			player.transform.position = spawnMarker.SpawnPosition;
		}

		//After player data is loaded, load the level
		LoadLevel(firstSceneName, null);
    }

	private SpawnMarker FindSpawnMarker(string spawnMarkerName)
	{
		//Find doorway and place at doorway out position
		GameObject[] spawnMarkers = GameObject.FindGameObjectsWithTag("SpawnMarker");

		if (spawnMarkers.Length < 1)
			Debug.LogError("No doors found!");

		foreach (GameObject marker in spawnMarkers)
		{
			SpawnMarker d = marker.GetComponent<SpawnMarker>();

			if (marker.name == spawnMarkerName)
			{
				return d;
			}
		}

		Debug.LogError($"Target Doorway: {spawnMarkerName} not found!");

		return null;
	}
}
