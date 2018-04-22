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

    //Assembled save location with slot number and editor extension
    public string SaveLocation { get { return string.Format("{0}/Save{1}.dat", Application.persistentDataPath, saveSlot); } }

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
        //Only save data if there is a player
        if (player && data != null)
        {
            //Get location data for player (level and position)
            SaveData.Location location = new SaveData.Location(GameManager.instance.loadedSceneIndex, player.transform.position);

            //Always update location for autosave
            data.autoSave = location;

            //Only update location for hardsave when saved via NPC
            if (hardSave)
                data.npcSave = location;

            CharacterStats stats = player.GetComponent<CharacterStats>();
            PlayerAttack attack = player.GetComponent<PlayerAttack>();
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
			PlayerMagicBank bank = player.GetComponent<PlayerMagicBank>();

            if (stats)
            {
                //Store player stats
                data.maxHealth = stats.maxHealth;
                data.currentHealth = stats.currentHealth;
            }

            if(attack)
            {
				data.magicProgression = attack.magicProgression;

				data.selectedElement = attack.selectedElement;

				data.hasFireMagic = attack.hasFireMagic;
				data.hasGrassMagic = attack.hasGrassMagic;
				data.hasIceMagic = attack.hasIceMagic;
				data.hasWindMagic = attack.hasWindMagic;
			}

            if(inventory)
            {
				List<string> names = new List<string>((inventory.items.Count));

				foreach(InventoryItem item in inventory.items)
					names.Add(item.name);

                data.inventoryItems = names;
				inventory.UpdateInventory();
            }

			if(bank)
			{
				data.maxSouls = bank.maxSouls;

				data.currentFireSouls = bank.currentFireSouls;
				data.currentGrassSouls = bank.currentGrassSouls;
				data.currentIceSouls = bank.currentIceSouls;
				data.currentWindSouls = bank.currentWindSouls;
			}

            //Serialise save data to JSON
            string saveString = JsonUtility.ToJson(data, true);

            //Write serialised data to file
            System.IO.File.WriteAllText(SaveLocation, saveString);
        }
        else
            Debug.LogWarning("Save Manager could not find player! Saving did not work.");
    }

    public bool LoadGame()
    {
        //Load existing save data first, if any
        if (System.IO.File.Exists(SaveLocation))
        {
            string loadString = System.IO.File.ReadAllText(SaveLocation);
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
                if (System.IO.Path.GetExtension(file) == ".dat")
                {
                    System.IO.File.Delete(file);

                    fileCount++;
                }
            }

            Debug.Log("Save Files deleted: " + fileCount);
        }
    }

	public bool GetBlackboardJson(string key, out string json)
	{
		if (data.blackboardDictionary.ContainsKey(key))
		{
			json = data.blackboardDictionary[key];
			return true;
		}
		else
		{
			json = "";
			return false;
		}
	}

	public void SaveBlackBoardJson(string key, string json)
	{
		data.blackboardDictionary[key] = json;
	}

	public bool GetPersistentObjectState(string sceneName, string id)
	{
		if(data.persistentObjects.ContainsKey(sceneName))
		{
			var sceneDictionary = data.persistentObjects[sceneName];

			if(sceneDictionary.ContainsKey(id))
			{
				bool activated = sceneDictionary[id];
				return activated;
			}
		}

		return false;
	}

	public void SetPersistentObjectState(string sceneName, string id, bool activated)
	{
		//Get existing scene-level dictionary or create one
		SaveData.PersistentObjectIDDictionary sceneDictionary = null;
		if (data.persistentObjects.ContainsKey(sceneName))
			sceneDictionary = data.persistentObjects[sceneName];
		else
			sceneDictionary = data.persistentObjects[sceneName] = new SaveData.PersistentObjectIDDictionary();

		sceneDictionary[id] = activated;
	}
}
