using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform playerSpawn;
    public GameObject playerPrefab;

    private GameObject player;

    [Space()]
    public float playerRespawnDelay = 3f;

    //Game running refers to if events can happen in game. Inside menus the game is considered "not running"
    [Space()]
    public bool gameRunning = true;

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
                stats.OnDeath += Reload;
            }
        }
        else
            Debug.LogWarning("No player spawn assigned to Game Manager");
    }

    public void Reload()
    {
        StartCoroutine("ReloadWithDelay", playerRespawnDelay);
    }

    IEnumerator ReloadWithDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
