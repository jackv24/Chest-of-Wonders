using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class InventoryUI : NotebookPageUI
{
	public InventoryUISlot[] slots;

	public int inventoryRowSplit = 4;

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
			UIGridSlot.LinkNavigation(slots, inventoryRowSplit, menuButton);

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
}
