using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class InventoryUI : NotebookPageGridUI
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

		SetupUI(slots, inventoryRowSplit);
	}

	protected override void UpdateUI()
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
