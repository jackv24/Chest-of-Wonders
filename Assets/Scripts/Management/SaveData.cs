using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Location
    {
        public string sceneName;
        public Vector2 position;

        public Location(string sceneName, Vector2 position)
        {
            this.sceneName = sceneName;
            this.position = position;
        }
    }

    //Location data
    public Location autoSave; //Used when reloading save
    public Location npcSave; //Used when respawning after death

    //Player data
    public int maxHealth;
    public int currentHealth;

    public List<InventoryItem> inventory;

	//Lists of unique IDs for keeping objects disabled
    public List<int> pickedUpItems;
	public List<int> openedDoors;
	public List<int> pulledSwitches;

	[System.Serializable]
	public class ObjectPositionDictionary : SerializableDictionary<int, Vector2> { }

	//Dictionary of moved item positions
	public ObjectPositionDictionary savedObjectPositions;

	[System.Serializable]
    public class DialogueDictionary : SerializableDictionary<string, string> { }

	//Dictionary of saved dialogue states
    public DialogueDictionary savedDialogue;

	public List<string> flags;
}
