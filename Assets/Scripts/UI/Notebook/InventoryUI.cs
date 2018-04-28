using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class InventoryUI : NotebookPageUI
{
	public InventoryUISlot[] slots;

	public int inventoryRowSplit = 3;

	private PlayerInventory inventory;

	protected override void Close(bool quickHide)
	{
		gameObject.SetActive(false);
	}

	protected override void Open(bool quickShow)
	{
		gameObject.SetActive(true);

		if (slots.Length > 0)
		{
			//Update UI slots to show items in inventory
			UpdateUI();

			//Re-link navigation (in case any inventory items have been added/removed)
			LinkNavigation();

			//Re-link menu buttons with the first inventory slot mapped to right (if the slot can be mapped)
			PauseScreenUI.Instance.LinkMenuButtons(slots[0].Selectable ? (slots[0].Selectable.CanSelect() ? slots[0].Selectable : null) : null);
		}
	}

	private void UpdateUI()
    {
		if (!inventory)
			inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

		if (inventory)
        {
			int itemCount = inventory.items.Count;

            for(int i = 0; i < slots.Length; i++)
			{
				if (i < itemCount)
				{
					InventoryItem item = inventory.items[i];

					slots[i].SetItem(item);
				}
				else
					slots[i].SetItem(null);
			}
        }
    }

	private void LinkNavigation()
	{
		//Treat 1D array as 2D array for easier traversal (left as list for display in inspector)
		int width = inventoryRowSplit;
		int height = slots.Length / inventoryRowSplit;

		//Loop horizontally for each row
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				Selectable self = Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j).Selectable;

				Navigation nav = new Navigation();

				//NOTE:	We only bother testing if slots are interactable to the right and down since the
				//		inventory fills up from the top-right corner
				if (self.CanSelect())
				{
					nav.mode = Navigation.Mode.Explicit;

					///Horizontal navigation
					//Left should be the slot on left, unless this is the leftmost slot, then left is first menu button (if it exists)
					nav.selectOnLeft = i > 0 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i - 1, j).Selectable : menuButton;

					//Right should be the slot on the right (clamped & only if interactable)
					Selectable slotRight = i < width - 1 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i + 1, j).Selectable : null;
					nav.selectOnRight = slotRight ? (slotRight.CanSelect() ? slotRight : null) : null;

					///Vertical navigation
					//Slot above (clamped)
					nav.selectOnUp = j > 0 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j - 1).Selectable : null;

					//Slot below (clamped & only if interactable)
					Selectable slotBelow = j < height - 1 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j + 1).Selectable : null;
					nav.selectOnDown = slotBelow ? (slotBelow.CanSelect() ? slotBelow : null) : null;

				}
				else
					nav.mode = Navigation.Mode.None;

				self.navigation = nav;
			}
		}
	}
}
