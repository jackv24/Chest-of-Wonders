using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJournalManager : MonoBehaviour
{
	public static EnemyJournalManager Instance;

	[System.Serializable]
	public struct EnemyKillRecord
	{
		public int killCount;
		public bool hasReceivedAward;

		public static EnemyKillRecord New
		{
			get
			{
				return new EnemyKillRecord
				{
					killCount = 1,
					hasReceivedAward = false
				};
			}
		}
	}
	public Dictionary<EnemyJournalRecord, EnemyKillRecord> killedEnemies = new Dictionary<EnemyJournalRecord, EnemyKillRecord>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//Subscribe to save/load events
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				//Convert saved data into useful game data
				killedEnemies = new Dictionary<EnemyJournalRecord, EnemyKillRecord>(data.killedEnemies.Count);

				foreach(var pair in data.killedEnemies)
				{
					//Load record object by name for easy use in-game
					EnemyJournalRecord record = Resources.Load<EnemyJournalRecord>($"Enemy Journal Records/{pair.Key}");

					killedEnemies.Add(record, pair.Value);
				}
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				//Convert game data into suitable save data
				data.killedEnemies = new SaveData.EnemyKillDictionary(killedEnemies.Count);

				foreach(var pair in killedEnemies)
				{
					//Save ScriptableObject name since a reference to the actual object cannot be saved
					//NOTE: This is extremely sensitive to naming changes, do not change enemy record object names after release!
					data.killedEnemies.Add(pair.Key.name, pair.Value);
				}
			};
		}
	}

	public void RecordKill(EnemyJournalRecord record)
	{
		EnemyKillRecord killRecord = killedEnemies.ContainsKey(record) ? killedEnemies[record] : EnemyKillRecord.New;

		killRecord.killCount++;

		killedEnemies[record] = killRecord;
	}
}
