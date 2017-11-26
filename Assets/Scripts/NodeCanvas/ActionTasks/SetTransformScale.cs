using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
	public class SetTransformScale : ActionTask
	{
		public BBParameter<Vector3> scale = Vector3.one;

		public bool multiply = true;

		protected override void OnExecute()
		{
			if (multiply)
			{
				Vector3 localScale = agent.transform.localScale;

				localScale.x *= scale.value.x;
				localScale.y *= scale.value.y;
				localScale.z *= scale.value.z;

				agent.transform.localScale = localScale;
			}
			else
				agent.transform.localScale = scale.value;

			EndAction(true);
		}
	}
}
