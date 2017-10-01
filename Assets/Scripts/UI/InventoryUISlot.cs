using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryUISlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public Image itemIcon;

	public InventoryItem item;

	private Selectable selectable;

	private void Awake()
	{
		selectable = GetComponent<Selectable>();
	}

	public void SetItem(InventoryItem inventoryItem)
	{
		item = inventoryItem;

		if (itemIcon)
		{
			//If there is an item, show it and make this slot selectable, else don't
			if (item)
			{
				itemIcon.sprite = item.inventoryIcon;
				itemIcon.SetNativeSize();

				itemIcon.gameObject.SetActive(true);

				if (selectable)
					selectable.interactable = true;
			}
			else
			{
				itemIcon.gameObject.SetActive(false);

				if (selectable)
					selectable.interactable = false;
			}
		}
	}

	//UI events
	public void OnDeselect(BaseEventData eventData)
	{
		if (ItemTooltip.Instance)
			ItemTooltip.Instance.Hide(this);
	}

	public void OnSelect(BaseEventData eventData)
	{
		if(ItemTooltip.Instance)
			ItemTooltip.Instance.Show(this);
	}
}
