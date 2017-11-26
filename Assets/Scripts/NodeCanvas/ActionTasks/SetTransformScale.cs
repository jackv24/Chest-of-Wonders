using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	public class SetTransformScale : ActionTask<Transform>
	{
		public BBParameter<Vector3> scale = Vector3.one;

		public bool multiply = true;

		protected override void OnExecute()
		{
			if (multiply)
			{
				Vector3 localScale = agent.localScale;

				localScale.x *= scale.value.x;
				localScale.y *= scale.value.y;
				localScale.z *= scale.value.z;

				agent.localScale = localScale;
			}
			else
				agent.localScale = scale.value;

			EndAction(true);
		}
	}
}
