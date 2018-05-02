using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[CreateAssetMenu(fileName = "New Enemy Record", menuName = "Data/Enemy Journal Record")]
public class EnemyJournalRecord : ScriptableObject
{
	public LocalizedString displayName = "Enemy Name";
	public LocalizedString description;

	public Sprite slotIcon;
	public Sprite cardImage;

	public int killsRequired = 1;

	public void RecordKill()
	{
		if (!EnemyJournalManager.Instance)
		{
			Debug.LogError("No instance of EnemyJournalManager could be found!");
			return;
		}

		EnemyJournalManager.Instance.RecordKill(this);
	}
}
