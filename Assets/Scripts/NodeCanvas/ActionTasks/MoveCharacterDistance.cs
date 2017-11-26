using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	public class MoveCharacterDistance : ActionTask
	{
		public BBParameter<float> distance = 1.0f;
		public BBParameter<int> direction = 1;

		private CharacterMove character;
		private float initialPosX;
		private float targetPosX;

		protected override void OnExecute()
		{
			character = agent.GetComponent<CharacterMove>();

			if (character)
			{
				initialPosX = character.transform.position.x;
				targetPosX = character.transform.position.x + Mathf.Abs(distance.value) * Mathf.Sign(direction.value);
			}
			else
				EndAction(false);
		}

		protected override void OnUpdate()
		{
			if(character)
			{
				character.Move(Mathf.Sign(direction.value));

				if(Mathf.Abs(character.transform.position.x - initialPosX) >= Mathf.Abs(distance.value))
				{
					character.Move(0);
					EndAction(true);
				}
			}
		}
	}
}
