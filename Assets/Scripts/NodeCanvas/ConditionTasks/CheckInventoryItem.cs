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

		private PlayerInventory inventory;

		protected override string OnInit()
		{
			inventory = GameObject.FindObjectOfType<PlayerInventory>();

			return base.OnInit();
		}

		protected override bool OnCheck()
		{
			if (inventory && itemToCheck)
			{
				if (inventory.CheckItem(itemToCheck, consume))
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
