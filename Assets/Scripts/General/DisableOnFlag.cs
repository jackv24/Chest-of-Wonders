using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnFlag : MonoBehaviour
{
	public string flag;

	[Tooltip("If item is specified flag is ignored.")]
	public InventoryItem item;

	public bool enableOnFlag = false;

	private void Start()
	{
		bool disable = false;

		if(item)
		{
			PlayerInventory inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

			if(inventory)
				disable = inventory.CheckItem(item);
		}
		else
			disable = SaveManager.instance.CheckFlag(flag);

		if (enableOnFlag)
			disable = !disable;

		if(disable)
		{
			gameObject.SetActive(false);
		}
	}
}
