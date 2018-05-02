using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyJournalUI : NotebookPageUI
{
	public GridLayoutGroup slotsParent;
	private EnemyJournalUISlot[] slots = null;

	public EnemyJournalUICard cardDisplay;

	protected override void Close(bool quickHide)
	{
		cardDisplay?.Hide();

		gameObject.SetActive(false);
	}

	protected override void Open(bool quickShow)
	{
		gameObject.SetActive(true);

		if(slots == null)
		{
			if (!slotsParent)
				Debug.LogError("Slots parent needs to be assigned in Enemy Journal UI!");

			slots = slotsParent.GetComponentsInChildren<EnemyJournalUISlot>();
		}

		if (slots.Length > 0)
		{
			UpdateUI();

			UIGridSlot.LinkNavigation(slots, slotsParent.constraintCount, menuButton);

			//Re-link menu buttons with the first slot mapped to right (if the slot can be mapped)
			PauseScreenUI.Instance.LinkMenuButtons(slots[0].Selectable ? (slots[0].Selectable.CanSelect() ? slots[0].Selectable : null) : null);
		}
	}

	private void UpdateUI()
	{
		foreach(var slot in slots)
		{
			//Set slot image, check if unlocked, etc
			slot.SetDisplay();

			//Setup card display handling here, since slots don't need to know about it
			slot.OnSelection = (enemy) =>
			{
				cardDisplay?.Show(enemy);
			};

			slot.OnDeselection = () =>
			{
				cardDisplay?.Hide();
			};
		}
	}
}
