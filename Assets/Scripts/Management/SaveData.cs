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

	public bool hasFireMagic;
	public bool hasGrassMagic;
	public bool hasIceMagic;
	public bool hasWindMagic;

	public PlayerAttack.MagicProgression magicProgression;
	public ElementManager.Element selectedElement;

	public List<string> inventoryItems;

	//Magic bank
	public int maxSouls;

	public int currentFireSouls;
	public int currentGrassSouls;
	public int currentIceSouls;
	public int currentWindSouls;

	[System.Serializable]
	public class DialogueDictionary : SerializableDictionary<string, string> { }
	public DialogueDictionary blackboardDictionary;

	[System.Serializable]
	public class PersistentObjectIDDictionary : SerializableDictionary<string, bool> { }
	[System.Serializable]
	public class PersistentObjectDictionary : SerializableDictionary<string, PersistentObjectIDDictionary> { }
	public PersistentObjectDictionary persistentObjects;
}
