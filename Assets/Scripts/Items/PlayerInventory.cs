using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public delegate void NormalEvent();
    public NormalEvent OnUpdateInventory;

    public List<InventoryItem> items = new List<InventoryItem>();

	public void UpdateInventory()
	{
		if (OnUpdateInventory != null)
			OnUpdateInventory();
	}

	public void AddItem(InventoryItem item)
    {
        items.Add(item);

        if (OnUpdateInventory != null)
            OnUpdateInventory();
    }

	public bool CheckItem(InventoryItem item, bool consume = false)
	{
		bool contains = items.Contains(item);

		if(contains && consume)
		{
			items.Remove(item);

			if (OnUpdateInventory != null)
				OnUpdateInventory();
		}

		return contains;
	}
}
