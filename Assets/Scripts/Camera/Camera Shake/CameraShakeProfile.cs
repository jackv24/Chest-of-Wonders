using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Camera/Camera Shake Profile")]
public class CameraShakeProfile : ScriptableObject
{
	[SerializeField]
	private AnimationCurve decay = AnimationCurve.Constant(0, 1, 1);

	[SerializeField]
	private float duration = 1.0f;
	public float Duration { get { return duration; } }

	[SerializeField]
	private float magnitude = 1.0f;

	[SerializeField]
	private int freezeFrames = 1;
	public int FreezeFrames { get { return freezeFrames; } }

	public Vector2 GetOffset(float time)
	{
		return Random.insideUnitCircle * magnitude * decay.Evaluate(time);
	}
}
