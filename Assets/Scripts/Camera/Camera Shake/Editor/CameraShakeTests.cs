using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CameraShakeTests
{
	private class DummyCameraShake : ICameraShakeHandler
	{
		public CameraShakeProfile LastProfile { get; private set; }

		public void DoShake(CameraShakeProfile profile)
		{
			LastProfile = profile;
		}
	}

	[Test]
	public void CameraShakeReferenceDispatcheShakeWithProfile()
	{
		var profile = ScriptableObject.CreateInstance<CameraShakeProfile>();
		var reference = ScriptableObject.CreateInstance<CameraShakeReference>();
		var camShake = new DummyCameraShake();

		reference.RegisterCamera(camShake);
		reference.DoShake(profile);

		Assert.AreSame(camShake.LastProfile, profile);
	}

	[Test]
	public void CameraShakeTargetDispatchesShake()
	{
		var profile = ScriptableObject.CreateInstance<CameraShakeProfile>();
		var reference = ScriptableObject.CreateInstance<CameraShakeReference>();
		var camShake = new DummyCameraShake();
		var target = new CameraShakeTarget(reference, profile);

		reference.RegisterCamera(camShake);
		target.DoShake();

		Assert.AreSame(camShake.LastProfile, profile);
	}
}
