using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;

    public GameObject itemIconTemplate;

    private List<GameObject> itemIcons = new List<GameObject>();

    void Start()
    {
        if(!inventory)
            inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

        if (inventory)
            inventory.OnUpdateInventory += UpdateUI;

        if (itemIconTemplate)
            itemIconTemplate.SetActive(false);

        UpdateUI();
    }

    void UpdateUI()
    {
        if(itemIconTemplate && inventory)
        {
            //Disable all icons
            foreach(GameObject icon in itemIcons)
            {
                icon.SetActive(false);
            }

            for(int i = 0; i < inventory.items.Count; i++)
            {
                GameObject itemIcon = null;

                //Reuse any existing icons
                if (i < itemIcons.Count)
                    itemIcon = itemIcons[i];
                else
                {
                    //If adequate icon does not exist, instantiate a new one and it it to the list for later reuse
                    itemIcon = (GameObject)Instantiate(itemIconTemplate, itemIconTemplate.transform.parent);
                    itemIcons.Add(itemIcon);
                }

                //Enable icon and set sprite
                itemIcon.SetActive(true);
                itemIcon.GetComponent<Image>().sprite = inventory.items[i].inventoryIcon;
            }
        }
    }
}
