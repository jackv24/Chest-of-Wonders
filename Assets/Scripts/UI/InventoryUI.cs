using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;

	private InventoryUISlot[] slots;

	void Start()
    {
        if(!inventory)
            inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

		slots = GetComponentsInChildren<InventoryUISlot>();

		//Update inventory UI when game is paused, and UI is shown
		if (GameManager.instance)
			GameManager.instance.OnPausedChange += (bool value) => { if (value) UpdateUI(); };

		//Update UI once on open, as game will have already been paused before event is subsscribed
		UpdateUI();
    }

    void UpdateUI()
    {
        if(inventory)
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
