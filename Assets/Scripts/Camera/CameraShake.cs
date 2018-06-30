using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraShake : MonoBehaviour
{
	public enum ShakeType
	{
		EnemyHit,
		EnemyKill,
		PlayerHit,
		PlayerKill
	}

	[System.Serializable]
	public class ShakeProfile
	{
		public AnimationCurve decay = AnimationCurve.Constant(0, 1, 1);
		public float duration = 1.0f;
		public float magnitude = 1.0f;
		public int freezeFrames = 1;

		private float elapsed = 0;

		public bool IsDone { get { return elapsed >= duration; } }

		public ShakeProfile GetNewCopy()
		{
			return new ShakeProfile()
			{
				decay = decay,
				duration = duration,
				magnitude = magnitude,
				elapsed = 0
			};
		}

		public void AddTime(float time)
		{
			elapsed += time;
		}

		public Vector2 GetOffset()
		{
			float time = elapsed / duration;

			return UnityEngine.Random.insideUnitCircle * magnitude * decay.Evaluate(time);
		}
	}

	[ArrayForEnum(typeof(ShakeType))]
	public ShakeProfile[] shakeProfiles;

	private List<ShakeProfile> currentShakes = new List<ShakeProfile>();
	private Vector3 initialPosition;

	private Coroutine freezeFrameRoutine = null;
	private Action onFreezeEnd;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref shakeProfiles, typeof(ShakeType));
	}

	private void OnPreCull()
	{
		initialPosition = transform.position;

		//Only bother evaluating shake when not in freeze frame
		if(freezeFrameRoutine == null)
			transform.position += (Vector3)EvaluateShakes();
	}

	private void OnPostRender()
	{
		//Return initial position after render to prevent shake from interfering with gameplay code
		transform.position = initialPosition;
	}

	public void DoShake(ShakeType type)
	{
		var profile = shakeProfiles[(int)type];

		//Add a copy of the shake profile to the list (since we'll be editing this copy)
		currentShakes.Add(profile.GetNewCopy());

		//Do freeze frames if desired
		if(profile.freezeFrames > 0)
		{
			//Stop and clean up after any current freeze frame before starting another
			if(freezeFrameRoutine != null)
			{
				StopCoroutine(freezeFrameRoutine);
				onFreezeEnd?.Invoke();
			}

			freezeFrameRoutine = StartCoroutine(FreezeFrames(profile.freezeFrames));
		}
	}

	private Vector2 EvaluateShakes()
	{
		Vector2 totalOffset = Vector2.zero;

		//Get compounded offset from all currently running shakes
		foreach (var shake in currentShakes)
		{
			shake.AddTime(Time.deltaTime);
			totalOffset += shake.GetOffset();
		}

		//Remove any shakes from list that are done (work backward through list to allow iterating while removing)
		for(int i = currentShakes.Count - 1; i >= 0; i--)
		{
			if (currentShakes[i].IsDone)
				currentShakes.RemoveAt(i);
		}

		return totalOffset;
	}

	private IEnumerator FreezeFrames(int frameCount)
	{
		float timeScale = Time.timeScale;
		Time.timeScale = 0;

		onFreezeEnd = () => { Time.timeScale = timeScale; };

		// Safe to approximate a frame, since we're just waiting for an amount of time
		// Will also prevent any consistency problems at differing framerates
		yield return new WaitForSecondsRealtime((1 / 60.0f) * frameCount);

		onFreezeEnd.Invoke();
		onFreezeEnd = null;

		freezeFrameRoutine = null;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraShake))]
public class CameraShakeEditor : Editor
{
	private CameraShake cameraShake;

	private void OnEnable()
	{
		cameraShake = (CameraShake)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();

		//Test buttons will only work while the game is playing
		if(Application.isPlaying)
		{
			EditorGUILayout.HelpBox("Remember to save your changes using Copy Component, then Paste Component Values after leaving play mode.", MessageType.Warning);

			var profiles = cameraShake.shakeProfiles;
			for(int i = 0; i < profiles.Length; i++)
			{
				string name = ObjectNames.NicifyVariableName(Enum.GetNames(typeof(CameraShake.ShakeType))[i]);

				if(GUILayout.Button(name))
				{
					cameraShake.DoShake((CameraShake.ShakeType)i);
				}
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Profiles can be tested in Play Mode", MessageType.Info);
		}
	}
}
#endif
