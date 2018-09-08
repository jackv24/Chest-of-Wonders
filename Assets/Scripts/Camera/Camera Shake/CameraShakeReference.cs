using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/Camera/Camera Shake Reference")]
public class CameraShakeReference : ScriptableObject
{
	[NonSerialized]
	private List<CameraShake> targetCameras;

	public void RegisterCamera(CameraShake cameraShake)
	{
		if (targetCameras == null)
			targetCameras = new List<CameraShake>();

		targetCameras.Add(cameraShake);
	}

	public void DeregisterCamera(CameraShake cameraShake)
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
