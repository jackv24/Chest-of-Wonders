using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Conditions
{
	public class CharacterCheckWall : ConditionTask
	{
		public BBParameter<CharacterMove> character;

		public BBParameter<float> direction;

		protected override bool OnCheck()
		{
			if(character.value)
			{
				//If hitting a wall but not moving in that direction, then not considered hitting
				if (character.value.HittingWall && Mathf.Sign(character.value.inputDirection) != Mathf.Sign(direction.value))
					return false;

				return character.value.HittingWall;
			}
			else
				return false;
		}
	}
}
