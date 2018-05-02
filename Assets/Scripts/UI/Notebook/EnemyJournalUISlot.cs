using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyJournalUISlot : UIGridSlot
{
	public delegate void OnSelectDelegate(EnemyJournalRecord enemy);
	public OnSelectDelegate OnSelection;

	public delegate void OnDeselectDelegate();
	public OnDeselectDelegate OnDeselection;

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
				imageDisplay.sprite = enemy.slotIcon;
				//imageDisplay.SetNativeSize(); //Will un-comment when actual icons are made for enemies

				//Set slot as unlocked if enemy killed (and if enemy assigned to slot) else locked
				imageDisplay.color = unlocked ? unlockedColor : lockedColor;
			}
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		if(unlocked)
			OnDeselection?.Invoke();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if (enemy && unlocked)
		{
			//Handle selection elsewhere since the slot does not need to know what happens after it's selected
			OnSelection?.Invoke(enemy);
		}
	}
}
