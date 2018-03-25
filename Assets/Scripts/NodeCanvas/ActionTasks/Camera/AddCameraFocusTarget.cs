using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Camera")]
	public class AddCameraFocusTarget : ActionTask<Transform>
	{
		protected override string info
		{
			get
			{
				return $"{(clearPrevious ? "Set" : "Add")} Camera Focus Target: {agent?.gameObject.name ?? "null"}";
			}
		}

		public Vector3 offset = Vector2.zero;
		public float weight = 1.0f;
		public bool clearPrevious = false;

		protected override void OnExecute()
		{
			CameraControl camera = CameraControl.Instance;

			if (camera)
			{
				if (clearPrevious)
					camera.ClearFocusTargets();

				camera.AddFocusTarget(new CameraControl.FocusTarget(agent, offset, weight));
			}

			EndAction(true);
		}
	}
}
