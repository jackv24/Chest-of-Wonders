using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public int saveSlot = 0;

    public SaveData data = null;
    private string saveLocation;

    private GameObject player;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //Concatenate save location with data path and save slot
        saveLocation = string.Format("{0}/Save{1}.cow", Application.persistentDataPath, saveSlot);

        player = GameObject.FindWithTag("Player");
    }

    public void SaveGame(bool hardSave)
    {
        //Only save data if there is a player
        if (player && data != null)
        {
            //Get location data for player (level and position)
            SaveData.Location location = new SaveData.Location(GameManager.instance.loadedLevelIndex, player.transform.position);

            //Always update location for autosave
            data.autoSave = location;

            //Only update location for hardsave when saved via NPC
            if (hardSave)
                data.npcSave = location;

            CharacterStats stats = player.GetComponent<CharacterStats>();

            if (stats)
            {
                //Store player stats
                data.maxHealth = stats.maxHealth;
                data.currentHealth = stats.currentHealth;
            }
            
            //Serialise save data to JSON
            string saveString = JsonUtility.ToJson(data, true);

            //Write serialised data to file
            System.IO.File.WriteAllText(saveLocation, saveString);
        }
        else
            Debug.LogWarning("Save Manager could not find player! Saving did not work.");
    }

    public void LoadGame()
    {
        //Load existing save data first, if any
        if (System.IO.File.Exists(saveLocation))
        {
            string loadString = System.IO.File.ReadAllText(saveLocation);
            data = (SaveData)JsonUtility.FromJson(loadString, typeof(SaveData));
        }
        else
            data = new SaveData();
    }
}
