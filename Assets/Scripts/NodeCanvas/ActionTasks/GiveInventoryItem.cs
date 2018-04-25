using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Player/Inventory")]
	public class GiveInventoryItem : ActionTask
	{
		public BBParameter<InventoryItem> itemToGive;

		protected override void OnExecute()
		{
			if(PlayerInventory.Instance && !itemToGive.isNone)
			{
				PlayerInventory.Instance.AddItem(itemToGive.value);
			}

			EndAction(true);
		}
	}
}
