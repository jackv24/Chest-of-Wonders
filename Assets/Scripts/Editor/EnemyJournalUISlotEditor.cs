using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyJournalUISlot))]
public class EnemyJournalUISlotEditor : Editor
{
	private EnemyJournalUISlot slot;
	private EnemyJournalRecord oldEnemy;

	private void OnEnable()
	{
		slot = (EnemyJournalUISlot)target;
		oldEnemy = slot.enemy;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		base.OnInspectorGUI();

		//If enemy in slot has changed update name for easy viewing in heirarchy
		if (EditorGUI.EndChangeCheck() && slot.enemy != oldEnemy)
		{
			oldEnemy = slot.enemy;

			slot.gameObject.name = $"Slot ({(slot.enemy ? slot.enemy.name : "Empty")})";

			slot.SetDisplay(false);
		}
	}
}
