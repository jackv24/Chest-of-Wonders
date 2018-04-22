using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("GameObject")]
	public class GetGameObjectDirection : ActionTask<Transform>
	{
		public BBParameter<GameObject> gameObject;

		public BBParameter<float> directionVar;

		protected override void OnExecute()
		{
			if(gameObject.value)
			{
				float offsetX = gameObject.value.transform.position.x - agent.position.x;

				directionVar.value = Mathf.Sign(offsetX);
			}

			EndAction(true);
		}
	}
}
