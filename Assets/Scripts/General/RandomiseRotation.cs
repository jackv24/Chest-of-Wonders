using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseRotation : MonoBehaviour
{
	public MinMaxFloat range = new MinMaxFloat(0, 360.0f);

	public float snapAngle = 0;

	private void OnEnable()
	{
		float angle = range.RandomValue;

		if(snapAngle != 0)
		{
			angle = snapAngle * Mathf.Round(angle / snapAngle);
		}

		transform.SetRotationZ(angle);
	}
}
