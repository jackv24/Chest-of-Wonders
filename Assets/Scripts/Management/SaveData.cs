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

	//Attacks
    public MagicAttack attack1;
    public int mana1;

    public MagicAttack attack2;
    public int mana2;

    public List<InventoryItem> inventory;

	//Lists of unique IDs for keeping objects disabled
    public List<int> pickedUpItems;
	public List<int> openedDoors;

	[System.Serializable]
    public class DialogueDictionary : SerializableDictionary<string, string> { }

	//DIctionary of saved dialogue states
    public DialogueDictionary savedDialogue;
}
