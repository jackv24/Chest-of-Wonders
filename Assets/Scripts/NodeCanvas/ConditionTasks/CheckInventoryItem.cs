using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("Player/Inventory")]
	public class CheckInventoryItem : ConditionTask
	{
		public BBParameter<InventoryItem> itemToCheck;
		public BBParameter<bool> consume = false;

		protected override bool OnCheck()
		{
			if (PlayerInventory.Instance && !itemToCheck.isNone)
			{
				if (PlayerInventory.Instance.CheckItem(itemToCheck.value, consume.value))
				{
					return true;
				}
				else
					return false;
			}
			else
				return base.OnCheck();
		}
	}
}
