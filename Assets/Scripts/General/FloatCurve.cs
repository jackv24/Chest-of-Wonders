using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatCurve : MonoBehaviour
{
	public AnimationCurve moveX;
	public float magnitudeX = 0.5f;
	public float animLengthX = 2.0f;
	public float timeOffsetX = 0;
	[Space()]
	public AnimationCurve moveY;
	public float magnitudeY = 0.5f;
	public float animLengthY = 2.0f;
	public float timeOffsetY = 0.5f;

	private Vector2 initialPos;

	private void Start()
	{
		initialPos = transform.position;
	}

	private void Update()
	{
		Vector2 newPos = initialPos;

		newPos.x += moveX.Evaluate((Time.time + timeOffsetX) / animLengthX) * magnitudeX;
		newPos.y += moveY.Evaluate((Time.time + timeOffsetY) / animLengthY) * magnitudeY;

		transform.position = newPos;
	}
}
