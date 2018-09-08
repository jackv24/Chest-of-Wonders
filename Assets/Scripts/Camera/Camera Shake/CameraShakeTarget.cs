using UnityEngine;
using System;

[Serializable]
public class CameraShakeTarget
{
	[SerializeField]
	private CameraShakeReference camera;
	public CameraShakeReference Camera { get { return camera; } }

	[SerializeField]
	private CameraShakeProfile profile;
	public CameraShakeProfile Profile { get { return profile; } }

	public CameraShakeTarget() { }

	public CameraShakeTarget(CameraShakeReference camera, CameraShakeProfile profile)
	{
		this.camera = camera;
		this.profile = profile;
	}

	public void DoShake()
	{
		if (camera)
			camera.DoShake(profile);
	}
}
