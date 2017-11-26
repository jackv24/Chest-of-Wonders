using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	public class GetGameObjectDirection : ActionTask
	{
		public BBParameter<GameObject> gameObject;

		public BBParameter<float> directionVar;

		protected override void OnExecute()
		{
			if(gameObject.value)
			{
				float offsetX = gameObject.value.transform.position.x - agent.transform.position.x;

				directionVar.value = Mathf.Sign(offsetX);
			}

			EndAction(true);
		}
	}
}
