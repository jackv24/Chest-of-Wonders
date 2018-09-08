using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/Camera/Camera Shake Reference")]
public class CameraShakeReference : ScriptableObject
{
	[NonSerialized]
	private List<ICameraShakeHandler> targetCameras;

	public void RegisterCamera(ICameraShakeHandler cameraShake)
	{
		if (targetCameras == null)
			targetCameras = new List<ICameraShakeHandler>();

		targetCameras.Add(cameraShake);
	}

	public void DeregisterCamera(ICameraShakeHandler cameraShake)
	{
		if (targetCameras != null)
			targetCameras.Remove(cameraShake);
	}

	public void DoShake(CameraShakeProfile profile)
	{
		if(profile && targetCameras != null)
		{
			foreach (var camera in targetCameras)
				camera.DoShake(profile);
		}
	}
}
