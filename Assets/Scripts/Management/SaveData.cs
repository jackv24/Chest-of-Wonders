using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Location
    {
        public int sceneIndex;
        public Vector2 position;

        public Location(int sceneIndex, Vector2 position)
        {
            this.sceneIndex = sceneIndex;
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

	public PlayerAttack.MagicProgression currentMagicProgression;

	public ElementManager.Element baseMagicSelected;

	public bool baseFireObtained = false;
	public bool baseGrassObtained = false;
	public bool baseIceObtained = false;
	public bool baseWindObtained = false;

	public int maxMana = 100;

	public List<PlayerAttack.MixMagic> mixMagics;

	//Magic bank
	public int maxSouls;

	public int currentFireSouls;
	public int currentGrassSouls;
	public int currentIceSouls;
	public int currentWindSouls;

	[System.Serializable]
	public class DialogueDictionary : SerializableDictionary<string, string> { }
	public DialogueDictionary blackboardDictionary;

	public List<string> flags;

	[System.Serializable]
	public class PersistentObjectIDDictionary : SerializableDictionary<string, bool> { }
	[System.Serializable]
	public class PersistentObjectDictionary : SerializableDictionary<string, PersistentObjectIDDictionary> { }
	public PersistentObjectDictionary persistentObjects;

	#region TO BE DEPRECATED
	//Lists of unique IDs for keeping objects disabled
	public List<int> pickedUpItems;
	public List<int> openedDoors;
	public List<int> pulledSwitches;

	[System.Serializable]
	public class ObjectPositionDictionary : SerializableDictionary<int, Vector2> { }

	//Dictionary of moved item positions
	public ObjectPositionDictionary savedObjectPositions;
	#endregion
}
