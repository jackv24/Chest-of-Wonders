using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public delegate void NormalEvent();
    public NormalEvent OnUpdateInventory;

    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(InventoryItem item)
    {
        items.Add(item);

        if (OnUpdateInventory != null)
            OnUpdateInventory();
    }
}
