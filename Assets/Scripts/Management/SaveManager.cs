using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public int saveSlot = 0;

    [Space()]
    public SaveData defaultSaveData;

    [HideInInspector]
    public SaveData data = null;
    private string saveLocation;

    //Assembled save location with slot number and editor extension
    public string SaveLocation { get { return string.Format("{0}/Save{1}{2}.cow", Application.persistentDataPath, saveSlot, Application.isEditor ? "_editor" : ""); } }

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
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

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

            if(inventory)
            {
                data.inventory = inventory.items;
				inventory.UpdateInventory();
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
        saveLocation = SaveLocation;

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

	//Temporary function for demo
	public void ResetAll()
	{
		ClearSave(true);

		SceneManager.LoadScene(0);
	}

    public void ClearSave(bool clearAll)
    {
        if (!clearAll)
        {
            //Get file path to save slot
            string location = SaveLocation;

            //If the file exists, delete it
            if (System.IO.File.Exists(location))
            {
                System.IO.File.Delete(location);

                Debug.Log("Save data cleared in slot " + saveSlot);
            }
            else
                Debug.LogWarning("No save data exists in slot " + saveSlot);
        }
        else
        {
            string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath);

            int fileCount = 0;

            foreach (string file in files)
            {
                //Delete all .cow files in persistent storage
                if (System.IO.Path.GetExtension(file) == ".cow")
                {
                    System.IO.File.Delete(file);

                    fileCount++;
                }
            }

            Debug.Log("Save Files deleted: " + fileCount);
        }
    }

    public string LoadDialogueJson(string dialogueName)
    {
        if (data.savedDialogue.ContainsKey(dialogueName))
        {
            return data.savedDialogue[dialogueName];
        }
        else
            return "";
    }

    public void SaveDialogueJson(string name, string json)
    {
        data.savedDialogue[name] = json;
    }

    public void SetPickedUpItem(int id)
    {
        data.pickedUpItems.Add(id);
    }

    public bool IsItemPickedUp(int id)
    {
        if (data.pickedUpItems.Contains(id))
            return true;
        else
            return false;
    }

	public void SetOpenedDoor(int id)
	{
		data.openedDoors.Add(id);
	}

	public bool IsDoorOpened(int id)
	{
		if (data.openedDoors.Contains(id))
			return true;
		else
			return false;
	}

	public void SetPulledSwitch(int id)
	{
		data.pulledSwitches.Add(id);
	}

	public bool IsSwitchPulled(int id)
	{
		if (data.pulledSwitches.Contains(id))
			return true;
		else
			return false;
	}

	public void SetObjectPosition(int id, Vector2 position)
	{
		data.savedObjectPositions[id] = position;
	}

	public Vector2 GetObjectPosition(int id, Vector2 defaultPos)
	{
		if (data.savedObjectPositions.ContainsKey(id))
		{
			return data.savedObjectPositions[id];
		}
		else
			return defaultPos;
	}

	public bool CheckFlag(string flag)
	{
		if (data.flags.Contains(flag))
			return true;
		else
			return false;
	}

	public void SetFlag(string flag)
	{
		if (!data.flags.Contains(flag))
			data.flags.Add(flag);
	}
}
