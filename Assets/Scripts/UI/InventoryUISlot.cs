using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
	public Image itemIcon;

	public void SetIcon(Sprite icon)
	{
		if (itemIcon)
		{
			if (icon)
			{
				itemIcon.sprite = icon;
				itemIcon.SetNativeSize();

				itemIcon.gameObject.SetActive(true);
			}
			else
				itemIcon.gameObject.SetActive(false);
		}
	}
}
