using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationFromBreakableObject : MonoBehaviour
{
	public enum Direction { Left, Right }
	public Direction defaultDirection = Direction.Right;

	public float altRotation;

	private float initialRotation;

	private void Awake()
	{
		initialRotation = transform.eulerAngles.z;
	}

	public void SetRotation(Vector2 direction)
	{
		if (direction == Vector2.zero)
			return;

		Direction dir = direction.x > 0 ? Direction.Right : Direction.Left;

		transform.SetRotationZ(dir == defaultDirection ? initialRotation : altRotation);
	}
}
