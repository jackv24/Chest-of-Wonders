using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[Description("Returns success if character is walking into a wall")]
	public class CharacterCheckWall : ConditionTask<CharacterMove>
	{
		public BBParameter<float> direction;

		protected override bool OnCheck()
		{
			//If hitting a wall but not moving in that direction, then not considered hitting
			if (agent.HittingWall && Mathf.Sign(agent.inputDirection) != Mathf.Sign(direction.value))
				return false;

			return agent.HittingWall;
		}
	}
}
