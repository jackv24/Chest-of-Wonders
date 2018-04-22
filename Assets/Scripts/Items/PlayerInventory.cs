using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	public static PlayerInventory Instance;

    public List<InventoryItem> items = new List<InventoryItem>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				items = new List<InventoryItem>(data.inventoryItems.Count);

				foreach (string name in data.inventoryItems)
					items.Add((InventoryItem)Resources.Load($"Items/{name}", typeof(InventoryItem)));
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				List<string> names = new List<string>((items.Count));

				foreach (InventoryItem item in items)
					names.Add(item.name);

				data.inventoryItems = names;
			};
		}
	}

	public void AddItem(InventoryItem item)
    {
        items.Add(item);
    }

	public bool CheckItem(InventoryItem item, bool consume = false)
	{
		bool contains = items.Contains(item);

		if(contains && consume)
		{
			items.Remove(item);
		}

		return contains;
	}

	public bool CheckItem(string name)
	{
		foreach(InventoryItem item in items)
		{
			if (item.displayName.ToLower() == name.ToLower())
				return true;
		}

		return false;
	}
}
