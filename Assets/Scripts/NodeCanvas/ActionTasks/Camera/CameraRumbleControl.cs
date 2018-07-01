using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodeCanvas.Tasks.Actions
{
	[Category("Camera")]
	[Name("Camera Rumble")]
	public class CameraRumbleControl : ActionTask
	{
		public bool stopRumble = false;

		public CameraShake.RumbleType type;

		protected override string info
		{
			get
			{
				if (stopRumble)
					return "Stop Camera Rumble";
				else
					return $"Start Camera Rumble: {type.ToString()}";
			}
		}

		protected override void OnExecute()
		{
			CameraShake camShake = CameraShake.Instance;

			if (camShake)
			{
				if (stopRumble)
					camShake.StopRumble();
				else
					camShake.StartRumble(type);
			}

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			stopRumble = EditorGUILayout.Toggle("Stop Rumble?", stopRumble);

			if(!stopRumble)
				type = (CameraShake.RumbleType)EditorGUILayout.EnumPopup("Rumble Type", type);
		}
#endif
	}
}
