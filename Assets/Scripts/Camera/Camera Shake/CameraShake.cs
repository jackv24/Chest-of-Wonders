using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraShake : MonoBehaviour, ICameraShakeHandler
{
	[SerializeField]
	private CameraShakeReference reference;

	private struct RunningCameraShake
	{
		public CameraShakeProfile Profile;
		public float Elapsed;

		public bool IsDone
		{
			get
			{
				if (Profile)
					return Elapsed >= Profile.Duration;

				return true;
			}
		}

		public Vector2 GetOffset()
		{
            if (Profile.Duration == 0)
                return Vector2.zero;

			return Profile?.GetOffset(Elapsed / Profile.Duration) ?? Vector2.zero;
		}
	}

	private List<RunningCameraShake> runningShakes = new List<RunningCameraShake>();

	private Vector3 initialPosition;

	private static int freezesRunning = 0;
    private float initialTimeScale;

	private void OnEnable()
	{
		reference?.RegisterCamera(this);
	}

	private void OnDisable()
	{
		reference?.DeregisterCamera(this);
	}

	private void OnPreCull()
	{
		initialPosition = transform.position;

		//Only bother evaluating shake when not in freeze frame
		if (freezesRunning <= 0)
		{
			transform.position += (Vector3)EvaluateShakes();
		}
	}

	private void OnPostRender()
	{
		//Return initial position after render to prevent shake from interfering with gameplay code
		transform.position = initialPosition;
	}

	public void DoShake(CameraShakeProfile profile)
	{
		//Add a copy of the shake profile to the list (since we'll be editing this copy)
		runningShakes.Add(new RunningCameraShake { Profile = profile });

		//Do freeze frames if desired
		if(profile.FreezeFrames > 0)
		{
			StartCoroutine(FreezeFrames(profile.FreezeFrames));
		}
	}

	private Vector2 EvaluateShakes()
	{
		Vector2 totalOffset = Vector2.zero;

		//Get compounded offset from all currently running shakes
		for(int i = 0; i < runningShakes.Count; i++)
		{
			var shake = runningShakes[i];
			shake.Elapsed += Time.deltaTime;
			totalOffset += shake.GetOffset();
			runningShakes[i] = shake;
		}

		//Remove any shakes from list that are done (work backward through list to allow iterating while removing)
		for(int i = runningShakes.Count - 1; i >= 0; i--)
		{
			if (runningShakes[i].IsDone)
				runningShakes.RemoveAt(i);
		}

		return totalOffset;
	}

	private IEnumerator FreezeFrames(int frameCount)
	{
        if (freezesRunning <= 0)
        {
            initialTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

		freezesRunning++;

		// Safe to approximate a frame, since we're just waiting for an amount of time
		// Will also prevent any consistency problems at differing framerates
		yield return new WaitForSecondsRealtime((1 / 60.0f) * frameCount);

		freezesRunning--;

		if (freezesRunning <= 0)
			Time.timeScale = initialTimeScale;
	}
}
