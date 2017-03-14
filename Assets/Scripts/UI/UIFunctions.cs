using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIFunctions : MonoBehaviour
{
    //The UI gameobject to enable to show the death screen
    public GameObject deathScreen;

    public GameObject pauseMenu;

    void Start()
    {
        //If there is a death screen object, make sure it is hidden by default
        if (deathScreen)
            deathScreen.SetActive(false);

        if (pauseMenu)
            pauseMenu.SetActive(false);

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

    public void ShowDeathScreen()
    {
        //If there is a death screen, enable it
        if (deathScreen)
            deathScreen.SetActive(true);
    }

    public void TogglePause()
    {
        GameManager.instance.TogglePaused();
    }

    public void ShowPauseMenu(bool value)
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(value);

            if (value)
                EventSystem.current.SetSelectedGameObject(pauseMenu.transform.GetComponentInChildren<Button>().gameObject);
        }
    }

    public void SaveGame()
    {
        //TODO: Code for saving game
    }

    public void LoadGame()
    {
        //TODO: Replace with actual game reload code
        Debug.Log("Reloading level yet to be implemented.");
    }
}
