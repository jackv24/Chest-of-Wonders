using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
	public static ItemTooltip Instance;

	public Text nameText;
	public Text descriptionText;

	public Vector2 offset = new Vector2(10, -10);

	private InventoryUISlot currentSlot;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		gameObject.SetActive(false);
	}

	public void Show(InventoryUISlot slot)
	{
		//Keep track of which slot called show
		currentSlot = slot;

		if (slot.item)
		{
			gameObject.SetActive(true);

			if (nameText)
				nameText.text = slot.item.displayName.TryGetTranslation();

			if (descriptionText)
				descriptionText.text = slot.item.description.TryGetTranslation();

			transform.position = (Vector2)slot.transform.position + offset;
		}
	}

	public void Hide(InventoryUISlot slot)
	{
		//Only allow hiding if the slot that called Hide is the slot that called Show
		if(slot == currentSlot)
		{
			gameObject.SetActive(false);
		}
	}
}
