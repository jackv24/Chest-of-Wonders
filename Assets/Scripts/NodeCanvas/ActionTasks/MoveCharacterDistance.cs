using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Moves the character a specified distance in a specified direction")]
	public class MoveCharacterDistance : ActionTask<CharacterMove>
	{
		public BBParameter<float> distance = 1.0f;
		public BBParameter<int> direction = 1;

		private float initialPosX;
		private float targetPosX;

		protected override void OnExecute()
		{
			initialPosX = agent.transform.position.x;
			targetPosX = agent.transform.position.x + Mathf.Abs(distance.value) * Mathf.Sign(direction.value);
		}

		protected override void OnUpdate()
		{
			agent.Move(Mathf.Sign(direction.value));

			if(Mathf.Abs(agent.transform.position.x - initialPosX) >= Mathf.Abs(distance.value))
			{
				agent.Move(0);
				EndAction(true);
			}
		}
	}
}
