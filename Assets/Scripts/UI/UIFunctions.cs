using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFunctions : MonoBehaviour
{
    //The UI gameobject to enable to show the death screen
    public GameObject deathScreen;

    void Start()
    {
        //If there is a death screen object, make sure it is hidden by default
        if (deathScreen)
            deathScreen.SetActive(false);

        //Subscribe show death screen to Game Over event
        GameManager.instance.OnGameOver += ShowDeathScreen;
    }

    public void LoadScene(int index)
    {
        //Cached GameObjects not valid between scene loads
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

    public void SaveGame()
    {
        //TODO: Code for saving game
    }

    public void LoadGame()
    {
        //TODO: Replace with actual game reload code
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
