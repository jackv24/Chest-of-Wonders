using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public int saveSlot = 0;

    [Space()]
    public SaveData defaultSaveData;

    [HideInInspector]
    public SaveData data = null;
    private string saveLocation;

    private GameObject player;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void SaveGame(bool hardSave)
    {
        if(saveLocation == null)
        {
            Debug.Log("Save data NOT loaded, since level was open on startup");
            return;
        }

        //Only save data if there is a player
        if (player && data != null)
        {
            //Get location data for player (level and position)
            SaveData.Location location = new SaveData.Location(GameManager.instance.loadedSceneName, player.transform.position);

            //Always update location for autosave
            data.autoSave = location;

            //Only update location for hardsave when saved via NPC
            if (hardSave)
                data.npcSave = location;

            CharacterStats stats = player.GetComponent<CharacterStats>();
            PlayerAttack attack = player.GetComponent<PlayerAttack>();

            if (stats)
            {
                //Store player stats
                data.maxHealth = stats.maxHealth;
                data.currentHealth = stats.currentHealth;
            }

            if(attack)
            {
                data.attack1 = attack.magicSlot1.attack;
                data.mana1 = attack.magicSlot1.currentMana;

                data.attack2 = attack.magicSlot2.attack;
                data.mana2 = attack.magicSlot2.currentMana;
            }
            
            //Serialise save data to JSON
            string saveString = JsonUtility.ToJson(data, true);

            //Write serialised data to file
            System.IO.File.WriteAllText(saveLocation, saveString);
        }
        else
            Debug.LogWarning("Save Manager could not find player! Saving did not work.");
    }

    public bool LoadGame()
    {
        //Concatenate save location with data path and save slot
        saveLocation = string.Format("{0}/Save{1}.cow", Application.persistentDataPath, saveSlot);

        //Load existing save data first, if any
        if (System.IO.File.Exists(saveLocation))
        {
            string loadString = System.IO.File.ReadAllText(saveLocation);
            data = (SaveData)JsonUtility.FromJson(loadString, typeof(SaveData));

            return true;
        }
        else
        {
            data = defaultSaveData;

            return true;
        }
    }

    public void ClearSave()
    {
        //Get file path to save slot
        string location = string.Format("{0}/Save{1}.cow", Application.persistentDataPath, saveSlot);

        //If the file exists, delete it
        if (System.IO.File.Exists(location))
        {
            System.IO.File.Delete(location);

            Debug.Log("Save data cleared in slot " + saveSlot);
        }
        else
            Debug.LogWarning("No save data exists in slot " + saveSlot);
    }
}
