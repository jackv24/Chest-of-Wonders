using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[CreateAssetMenu(fileName ="NewItem", menuName ="Data/Item")]
public class InventoryItem : ScriptableObject
{
    public LocalizedString displayName;
	public LocalizedString description;

    public Sprite inventoryIcon;
}
