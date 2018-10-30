using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryUISlot : UIGridSlot
{
	public Image itemIcon;

	private InventoryItem item;

	public void SetItem(InventoryItem inventoryItem)
	{
		//Make sure references have been gotten first, since it's possible for this method to be called before Awake
		Setup();

		item = inventoryItem;

		if (itemIcon)
		{
			//If there is an item, show it and make this slot selectable, else don't
			if (item)
			{
				itemIcon.sprite = item.InventoryIcon;
				itemIcon.SetNativeSize();

				itemIcon.gameObject.SetActive(true);

				if (Selectable)
					Selectable.interactable = true;
			}
			else
			{
				itemIcon.gameObject.SetActive(false);

				if (Selectable)
					Selectable.interactable = false;
			}
		}
	}

	//UI events
	public override void OnDeselect(BaseEventData eventData)
	{
		if(item)
			ItemTooltip.Instance?.Hide();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if(item)
			ItemTooltip.Instance?.Show(item.DisplayName, item.Description, transform.position);
	}
}
