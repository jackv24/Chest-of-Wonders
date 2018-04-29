using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyJournalUISlot : UIGridSlot
{
	public EnemyJournalRecord enemy;

	public Image imageDisplay;

	public Color unlockedColor = Color.white;
	public Color lockedColor = Color.grey;

	private bool unlocked = false;

	private void Awake()
	{
		Setup();
	}

	public void SetDisplay(bool checkIfKilled = true)
	{
		if(enemy)
		{
			unlocked = checkIfKilled ? (EnemyJournalManager.Instance?.HasKilled(enemy) ?? false) : true;

			if (imageDisplay)
			{
				imageDisplay.sprite = enemy.sprite;
				//imageDisplay.SetNativeSize(); //Will un-comment when actual icons are made for enemies

				//Set slot as unlocked if enemy killed (and if enemy assigned to slot) else locked
				imageDisplay.color = unlocked ? unlockedColor : lockedColor;
			}
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		ItemTooltip.Instance.Hide();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		//TODO: Replace with card UI
		if(enemy && unlocked)
			ItemTooltip.Instance.Show(enemy.displayName, enemy.description, transform.position);
		else
			ItemTooltip.Instance.Show("???", "???", transform.position);
	}
}
