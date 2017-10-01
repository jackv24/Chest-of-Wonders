using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem", menuName ="Data/Item")]
public class InventoryItem : ScriptableObject
{
    public string displayName;

    public Sprite inventoryIcon;

	[Multiline()]
	public string description;
}
