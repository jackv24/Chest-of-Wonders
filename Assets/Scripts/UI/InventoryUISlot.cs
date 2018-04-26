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

	public Selectable Selectable { get; private set; }

	private bool isSetup = false;

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if (isSetup)
			return;
		isSetup = true;

		Selectable = GetComponent<Selectable>();
	}

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
				itemIcon.sprite = item.inventoryIcon;
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
