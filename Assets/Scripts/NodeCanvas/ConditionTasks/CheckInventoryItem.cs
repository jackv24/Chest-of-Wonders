using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	public class CheckInventoryItem : ConditionTask
	{
		public InventoryItem itemToCheck;
		public bool consume = false;

		protected override bool OnCheck()
		{
			if (PlayerInventory.Instance && itemToCheck)
			{
				if (PlayerInventory.Instance.CheckItem(itemToCheck, consume))
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
