using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerInventory : MonoBehaviour
{
	public static PlayerInventory Instance;

    private Dictionary<InventoryItem, InventoryItemRuntimeData> items;

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
                items = data.InventoryItems
                .GroupBy(d => d.ItemName)
                .ToDictionary(
                    group => Resources.Load<InventoryItem>($"Items/{group.Key}"),
                    group => new InventoryItemRuntimeData(group.First())
                    );
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
                data.InventoryItems = items
                .Select(kvp => new InventoryItemSaveData(kvp.Key.name, kvp.Value))
                .ToList();
			};
		}
	}

	public void AddItem(InventoryItem item)
    {
        if (!items.ContainsKey(item))
            items[item] = new InventoryItemRuntimeData();

        var data = items[item];
        data.Amount++;
        items[item] = data;
    }

    public void RemoveItem(InventoryItem item)
    {
        if (!items.ContainsKey(item) || items[item].Amount <= 0)
            return;

        var data = items[item];
        data.Amount--;
        items[item] = data;
    }

	public bool CheckItem(InventoryItem item, bool consume = false)
	{
		bool contains = items.ContainsKey(item);
		if(contains && consume)
		{
			items.Remove(item);
		}

		return contains;
	}

    public int GetItemCount(InventoryItem item)
    {
        if (!items.ContainsKey(item))
            return -1;

        return items[item].Amount;
    }

    public List<InventoryItem> GetItems()
    {
        return items
            .Select(kvp => kvp.Key)
            .ToList();
    }
}

[Serializable]
public struct InventoryItemSaveData
{
    public string ItemName;
    public int Amount;

    public InventoryItemSaveData(string itemName, InventoryItemRuntimeData runtimeData)
    {
        ItemName = itemName;
        Amount = runtimeData.Amount;
    }
}

public struct InventoryItemRuntimeData
{
    public int Amount;

    public InventoryItemRuntimeData(InventoryItemSaveData saveData)
    {
        Amount = saveData.Amount;
    }
}
