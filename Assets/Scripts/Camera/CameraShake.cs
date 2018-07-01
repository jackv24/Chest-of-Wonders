using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance;

	public enum ShakeType
	{
		EnemyHit,
		EnemyKill,
		PlayerHit,
		PlayerKill
	}

	[Serializable]
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

		public void AddTime(float deltaTime)
		{
			elapsed += deltaTime;
		}

		public Vector2 GetOffset()
		{
			float time = elapsed / duration;

			return UnityEngine.Random.insideUnitCircle * magnitude * decay.Evaluate(time);
		}
	}

	public enum RumbleType
	{
		Small, Medium, Large
	}

	[Serializable]
	public class RumbleProfile
	{
		public float magnitude = 1.0f;

		public AnimationCurve endDecay;
		public float endDecayDuration = 1.0f;
		private float endDecayElapsed = 0;

		public bool IsDone { get { return endDecayElapsed >= endDecayDuration; } }

		public void AddDecayTime(float deltaTime)
		{
			endDecayElapsed += deltaTime;
		}

		public void ResetDecay()
		{
			endDecayElapsed = 0;
		}

		public Vector2 GetOffset()
		{
			float multiplier = 1.0f;

			if (endDecayElapsed > 0)
				multiplier = endDecay.Evaluate(endDecayElapsed / endDecayDuration);

			return UnityEngine.Random.insideUnitCircle * magnitude * multiplier;
		}
	}

	[ArrayForEnum(typeof(ShakeType))]
	public ShakeProfile[] shakeProfiles;

	[ArrayForEnum(typeof(RumbleType))]
	public RumbleProfile[] rumbleProfiles;

	private List<ShakeProfile> currentShakes = new List<ShakeProfile>();
	private RumbleType? currentRumble = null;
	private bool isRumbleDecaying = false;
	private Vector3 initialPosition;

	private Coroutine freezeFrameRoutine = null;
	private Action onFreezeEnd;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref shakeProfiles, typeof(ShakeType));
		ArrayForEnumAttribute.EnsureArraySize(ref rumbleProfiles, typeof(RumbleType));
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnPreCull()
	{
		initialPosition = transform.position;

		//Only bother evaluating shake when not in freeze frame
		if (freezeFrameRoutine == null)
		{
			transform.position += (Vector3)EvaluateShakes();

			//Only need to evaluate one rumble (the latest)
			if (currentRumble != null)
			{
				RumbleProfile profile = rumbleProfiles[(int)currentRumble.Value];

				if (isRumbleDecaying)
					profile.AddDecayTime(Time.deltaTime);

				transform.position += (Vector3)profile.GetOffset();

				if(profile.IsDone)
				{
					currentRumble = null;
					isRumbleDecaying = false;
					profile.ResetDecay();
				}
			}
		}
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

	public void StartRumble(RumbleType type)
	{
		if (currentRumble != null)
			rumbleProfiles[(int)currentRumble].ResetDecay();

		isRumbleDecaying = false;

		currentRumble = type;
	}

	public void StopRumble()
	{
		isRumbleDecaying = true;
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

		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Testing", EditorStyles.boldLabel);

		//Show relevant info box for current mode
		if(Application.isPlaying)
			EditorGUILayout.HelpBox("Remember to save your changes using Copy Component, then Paste Component Values after leaving play mode.", MessageType.Warning);
		else
			EditorGUILayout.HelpBox("Profiles can be tested in Play Mode", MessageType.Info);

		EditorGUILayout.LabelField("Shake", EditorStyles.centeredGreyMiniLabel);

		var shakeProfiles = cameraShake.shakeProfiles;
		for (int i = 0; i < shakeProfiles.Length; i++)
		{
			string name = ObjectNames.NicifyVariableName(Enum.GetNames(typeof(CameraShake.ShakeType))[i]);

			if (GUILayout.Button(name) && Application.isPlaying)
			{
				cameraShake.DoShake((CameraShake.ShakeType)i);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Rumble", EditorStyles.centeredGreyMiniLabel);

		var rumbleProfiles = cameraShake.rumbleProfiles;
		for (int i = 0; i < rumbleProfiles.Length; i++)
		{
			string name = ObjectNames.NicifyVariableName(Enum.GetNames(typeof(CameraShake.RumbleType))[i]);

			if (GUILayout.Button(name) && Application.isPlaying)
			{
				cameraShake.StartRumble((CameraShake.RumbleType)i);
			}
		}

		if (GUILayout.Button("Stop", EditorStyles.miniButton) && Application.isPlaying)
		{
			cameraShake.StopRumble();
		}

		EditorGUILayout.EndVertical();
	}
}
#endif
