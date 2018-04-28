using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJournalUI : NotebookPageUI
{
	public EnemyJournalUISlot[] slots;

	public int gridRowSplit = 4;

	protected override void Close(bool quickHide)
	{
		gameObject.SetActive(false);
	}

	protected override void Open(bool quickShow)
	{
		gameObject.SetActive(true);

		if (slots.Length > 0)
		{
			UpdateUI();

			UIGridSlot.LinkNavigation(slots, gridRowSplit, menuButton);

			//Re-link menu buttons with the first slot mapped to right (if the slot can be mapped)
			PauseScreenUI.Instance.LinkMenuButtons(slots[0].Selectable ? (slots[0].Selectable.CanSelect() ? slots[0].Selectable : null) : null);
		}
	}

	private void UpdateUI()
	{
		//Update slots
		foreach(var slot in slots)
		{
			slot.SetDisplay();
		}
	}
}
