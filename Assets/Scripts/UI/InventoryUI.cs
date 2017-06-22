using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;

    [Space()]
    public Text inventoryText;
    private string inventoryTextString;

    void Start()
    {
        if(!inventory)
            inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

        if (inventory)
            inventory.OnUpdateInventory += UpdateUI;

        if (inventoryText)
            inventoryTextString = inventoryText.text;

        UpdateUI();
    }

    void UpdateUI()
    {
        if(inventoryText && inventory)
        {
            Dictionary<InventoryItem, int> itemStrings = new Dictionary<InventoryItem, int>();

            foreach(InventoryItem item in inventory.items)
            {
                if (itemStrings.ContainsKey(item))
                    itemStrings[item]++;
                else
                    itemStrings.Add(item, 1);
            }

            string inventoryString = "";

            foreach (KeyValuePair<InventoryItem, int> item in itemStrings)
            {
                inventoryString += string.Format("{0}x{1}\n", item.Key.displayName, item.Value);
            }

            inventoryText.text = string.Format(inventoryTextString, inventoryString);
        }
    }
}
