using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectPerspectiveCamera : MonoBehaviour
{
	public const float pixelsPerUnit = 32.0f;

	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();

		UpdatePosition();
	}

	private void Update()
	{
		if(!Application.isPlaying)
		{
			UpdatePosition();
		}
	}

	private void UpdatePosition()
	{
		var targetFrustWidth = 360.0f / pixelsPerUnit;
		var frustumInnerAngles = (180f - cam.fieldOfView) / 2f * Mathf.PI / 180f;
		var newCamDist = Mathf.Tan(frustumInnerAngles) * (targetFrustWidth / 2);
		transform.SetLocalPositionZ(-newCamDist);
	}
}
