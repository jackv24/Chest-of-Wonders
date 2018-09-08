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
		public CameraShakeTarget cameraShake;

		protected override string info
		{
			get
			{
				string camera = cameraShake.Camera ? cameraShake.Camera.name : "None";
				string profile = cameraShake.Profile ? cameraShake.Profile.name : "None";

				return $"Shake Camera: {camera}, Profile: {profile}";
			}
		}

		protected override void OnExecute()
		{
			cameraShake.DoShake();

			EndAction(true);
		}
	}
}
