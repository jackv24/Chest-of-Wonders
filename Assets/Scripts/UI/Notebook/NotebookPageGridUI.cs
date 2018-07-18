using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

/// <summary>
/// Base class for notebook pages that implement selectable grids.
/// </summary>
public abstract class NotebookPageGridUI : NotebookPageUI
{
	protected void SetupUI(UIGridSlot[] slots, int gridRowSplit)
	{
		if (slots.Length > 0)
		{
			// Update UI slots to show items in inventory
			UpdateUI();

			// Re-link navigation (in case any inventory items have been added/removed)
			UIGridSlot.LinkNavigation(slots, gridRowSplit, menuButton);

			if (slots[0].Selectable && slots[0].Selectable.CanSelect())
			{
				// Re-link menu buttons with the first inventory slot mapped to right (if the slot can be mapped)
				PauseScreenUI.Instance.LinkMenuButtons(slots[0].Selectable);

				// Select the first slot on open
				slots[0].Selectable.Select();
			}
			else
			{
				// If no slot was available then clear any existing link
				PauseScreenUI.Instance.LinkMenuButtons(null);
			}
		}
	}

	protected abstract void UpdateUI();
}
