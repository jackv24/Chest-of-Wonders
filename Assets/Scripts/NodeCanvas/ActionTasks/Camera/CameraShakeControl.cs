using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Camera")]
	[Name("Camera Shake")]
	public class CameraShakeControl : ActionTask
	{
		public CameraShake.ShakeType type;

		protected override string info
		{
			get
			{
				return $"Shake Camera: {type.ToString()}";
			}
		}

		protected override void OnExecute()
		{
			CameraShake camShake = CameraShake.Instance;

			if (camShake)
			{
				camShake.DoShake(type);
			}

			EndAction(true);
		}
	}
}
