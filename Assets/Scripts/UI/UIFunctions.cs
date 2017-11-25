using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIFunctions : MonoBehaviour
{
    public static UIFunctions instance;

    //The UI gameobject to enable to show the death screen
    public GameObject deathScreen;
    public GameObject pauseMenu;
	public GameObject optionsMenu;
	public float pauseFadeTime = 0.25f;

    public GameObject loadingScreen;
    private bool loadingScreenActive = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //If there is a death screen object, make sure it is hidden by default
        if (deathScreen)
            deathScreen.SetActive(false);

        if (pauseMenu)
            pauseMenu.SetActive(false);

        if (loadingScreen)
            loadingScreen.SetActive(false);

        //Subscribe to events
        if (GameManager.instance)
        {
            GameManager.instance.OnGameOver += ShowDeathScreen;
            GameManager.instance.OnPausedChange += ShowPauseMenu;
        }
    }

    public void LoadScene(int index)
    {
        //Pooled objects are destroyed, so pools should be purged
        ObjectPooler.PurgePools();

        //Load the scene at a specified build index
        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowDeathScreen()
    {
        //If there is a death screen, enable it
        if (deathScreen)
        {
            deathScreen.SetActive(true);

            //Select first button
            GameObject obj = deathScreen.transform.GetComponentInChildren<Button>().gameObject;

			if (obj)
			{
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(obj);
			}
			else
				Debug.LogWarning("Could find a button to set selected!");
        }
    }

    public void TogglePause()
    {
        GameManager.instance.TogglePaused();
    }

    public void ShowPauseMenu(bool value)
    {
		if (optionsMenu && !value)
			optionsMenu.SetActive(false);

        if (pauseMenu)
        {
			GameObject selectObject = null;

			//Select first button if pause menu is shown
			if (value)
            {
                //Select first button
                selectObject = pauseMenu.transform.GetComponentInChildren<Button>().gameObject;

				if(!selectObject)
					Debug.LogWarning("Could find a button to set selected!");
			}

			//If there is a canvas group, fade it's alpha, otherwise just disable
			CanvasGroup screen = pauseMenu.GetComponent<CanvasGroup>();
			if (screen)
				StartCoroutine(FadeScreen(screen, pauseFadeTime, value ? 0 : 1, value ? 1 : 0, value, selectObject));
			else
				pauseMenu.SetActive(value);
		}
    }

	IEnumerator FadeScreen(CanvasGroup screen, float duration, float from, float to, bool setActive, GameObject selectObject)
	{
		float elapsed = 0;

		screen.alpha = from;

		if(setActive)
			screen.gameObject.SetActive(true);

		//Fade over time (realtime)
		while(elapsed < duration)
		{
			screen.alpha = Mathf.Lerp(from, to, elapsed / duration);

			yield return new WaitForEndOfFrame();
			elapsed += Time.unscaledDeltaTime;
		}

		screen.alpha = to;

		if (!setActive)
			screen.gameObject.SetActive(false);

		//Select desired object after fade in
		if(selectObject)
		{
			//If all buttons are deselected for some reason (such as if clicked in blank space), this allows navigating back
			EventSystem.current.firstSelectedGameObject = selectObject;

			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(selectObject);
		}
	}

    public void ShowLoadingScreen(bool value, float fadeDuration)
    {
        if(loadingScreen)
        {
            loadingScreenActive = value;
            StartCoroutine("FadeLoadingScreen", fadeDuration);
        }
    }

    IEnumerator FadeLoadingScreen(float duration)
    {
        Image img = loadingScreen.GetComponent<Image>();

        if(loadingScreenActive)
        {
            loadingScreen.SetActive(true);

            Color col = img.color;
            col.a = 0;
            float elapsedTime = 0;

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;

                col.a = Mathf.Lerp(0, 1, elapsedTime / duration);
                img.color = col;

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Color col = img.color;
            col.a = 1;
            float elapsedTime = 0;

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;

                col.a = Mathf.Lerp(1, 0, elapsedTime / duration);
                img.color = col;

                yield return new WaitForEndOfFrame();
            }

            loadingScreen.SetActive(false);
        }
    }

    public void SaveGame()
    {
        //TODO: Code for saving game
    }

    public void LoadGame()
    {
        //TODO: Replace with actual game load code
    }

    public void Respawn()
    {
        if(GameManager.instance)
        {
            GameManager.instance.SpawnPlayer(true);
            deathScreen.SetActive(false);
        }
    }

	public void ClearSaves()
	{
		
	}
}
