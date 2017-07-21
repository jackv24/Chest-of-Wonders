using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnFlag : MonoBehaviour
{
	public string flag;

	public InventoryItem item;

	public bool enableOnFlag = false;

	private void Start()
	{
		bool hasFlag = false;
		bool hasItem = false;

		if(item)
		{
			PlayerInventory inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

			if(inventory)
				hasItem = inventory.CheckItem(item);
		}

		if(flag != "")
			hasFlag = SaveManager.instance.CheckFlag(flag);

		bool disable = hasFlag || hasItem;

		if (enableOnFlag)
			disable = !disable;

		if(disable)
		{
			gameObject.SetActive(false);
		}
	}
}
