using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Location
    {
        public string SceneName;
        public string SpawnMarkerName;

        public Location(string sceneName, string spawnMarkerName)
        {
            SceneName = sceneName;
            SpawnMarkerName = spawnMarkerName;
        }
    }

    //Location data
    public Location AutoSave; //Used when reloading save
    public Location NpcSave; //Used when respawning after death

    //Player data
    public int MaxHealth;
    public int CurrentHealth;

	public bool HasFireMagic;
	public bool HasGrassMagic;
	public bool HasIceMagic;
	public bool HasWindMagic;

	public PlayerAttack.MagicProgression MagicProgression;
	public ElementManager.Element SelectedElement;

	public List<string> InventoryItems;

	//Magic bank
	public int MaxSouls;

	public int CurrentFireSouls;
	public int CurrentGrassSouls;
	public int CurrentIceSouls;
	public int CurrentWindSouls;

	[System.Serializable]
	public class EnemyKillDictionary : SerializableDictionary<string, EnemyJournalManager.EnemyKillRecord> { public EnemyKillDictionary(int capacity) : base(capacity) { } }
	public EnemyKillDictionary KilledEnemies;

	[System.Serializable]
	public class PersistentObjectDictionary : SerializableDictionary<string, bool> { }
	public PersistentObjectDictionary PersistentObjects = new PersistentObjectDictionary();

	[System.Serializable]
	public class DialogueDictionary : SerializableDictionary<string, string> { }
	public DialogueDictionary BlackboardDictionary = new DialogueDictionary();

	#region Accessor Functions

	public bool GetPersistentObjectState(string sceneName, string id)
	{
		if (PersistentObjects.ContainsKey(id))
		{
			bool activated = PersistentObjects[id];
			return activated;
		}

		return false;
	}

	public void SetPersistentObjectState(string sceneName, string id, bool activated)
	{
		PersistentObjects[id] = activated;
	}

	public bool GetBlackboardJson(string key, out string json)
	{
		if (BlackboardDictionary.ContainsKey(key))
		{
			json = BlackboardDictionary[key];
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
		BlackboardDictionary[key] = json;
	}

	#endregion
}
