﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Location
    {
        public string sceneName;
        public string spawnMarkerName;

        public Location(string sceneName, string spawnMarkerName)
        {
            this.sceneName = sceneName;
            this.spawnMarkerName = spawnMarkerName;
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
	public class EnemyKillDictionary : SerializableDictionary<string, EnemyJournalManager.EnemyKillRecord> { public EnemyKillDictionary(int capacity) : base(capacity) { } }
	public EnemyKillDictionary killedEnemies;

	[System.Serializable]
	public class DialogueDictionary : SerializableDictionary<string, string> { }
	public DialogueDictionary blackboardDictionary;

	[System.Serializable]
	public class PersistentObjectIDDictionary : SerializableDictionary<string, bool> { }
	[System.Serializable]
	public class PersistentObjectDictionary : SerializableDictionary<string, PersistentObjectIDDictionary> { }
	public PersistentObjectDictionary persistentObjects;
}
