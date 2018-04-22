using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

	public delegate void DataLoadedEvent(SaveData data);
	public event DataLoadedEvent OnDataLoaded;

	public delegate void DataSavedEvent(SaveData data, bool hardSave);
	public event DataSavedEvent OnDataSaving;

	public bool IsDataLoaded { get { return data != null; } }
	private SaveData data = null;

	public int saveSlot = 0;

    [Space()]
    public SaveData defaultSaveData;

    //Assembled save location with slot number and editor extension
    public string SaveLocation { get { return string.Format("{0}/Save{1}.dat", Application.persistentDataPath, saveSlot); } }

    void Awake()
    {
        instance = this;
    }

	private void Start()
	{
		//Will be overwritten if there is data to be loaded later
		data = defaultSaveData;
	}

	public void SaveGame(bool hardSave)
    {
		//Get subscribed objects to save their own state before saving file
		OnDataSaving?.Invoke(data, hardSave);

        //TODO: move int relevant classes
        if (data != null)
        {
            //Serialise save data to JSON
            string saveString = JsonUtility.ToJson(data, true);

            //Write serialised data to file
            System.IO.File.WriteAllText(SaveLocation, saveString);
        }
        else
            Debug.LogWarning("Tried to save null data. Save should never be called if load has not been called already!");
    }

    public void LoadGame(bool resetPlayerLocation)
    {
        //Load existing save data first, if any. Otherwise use default save data
        if (System.IO.File.Exists(SaveLocation))
        {
            string loadString = System.IO.File.ReadAllText(SaveLocation);
            data = (SaveData)JsonUtility.FromJson(loadString, typeof(SaveData));
        }

		//Subscribed object should process data after it has been loaded
		OnDataLoaded?.Invoke(data);
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
