using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Camera")]
	public class RemoveCameraFocusTarget : ActionTask<Transform>
	{
		protected override string info
		{
			get
			{
				return $"Remove Camera Focus Target: {agent?.gameObject.name ?? "null"}";
			}
		}

		protected override void OnExecute()
		{
			CameraControl camera = CameraControl.Instance;

			if (camera)
			{
				CameraControl.FocusTarget focusTarget = camera.GetFocusTargetByTransform(agent);
				if (focusTarget != null)
					camera.RemoveFocusTarget(focusTarget);
			}

			EndAction(true);
		}
	}
}
