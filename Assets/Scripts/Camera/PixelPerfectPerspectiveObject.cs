using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectPerspectiveObject : MonoBehaviour
{
	public const float pixelsPerUnit = PixelPerfectPerspectiveCamera.pixelsPerUnit;

	public Vector2 multiplier = Vector2.one;

	public bool updateEveryFrame = false;

	private void Start()
	{
		UpdateSize();
	}

	private void Update()
	{
		if(!Application.isPlaying || updateEveryFrame)
		{
			UpdateSize();
		}
	}

	private void UpdateSize()
	{
		//var a = Camera.main.WorldToScreenPoint(transform.position);
		//var b = new Vector3(a.x, a.y + pixelsPerUnit, a.z);

		//var aa = Camera.main.ScreenToWorldPoint(a);
		//var bb = Camera.main.ScreenToWorldPoint(b);

		//transform.localScale = multiplier * (aa - bb).magnitude;

		Camera cam = Camera.main;

		float cameraDistance = -cam.transform.position.z;
		float objectDistance = transform.position.z - cam.transform.position.z;

		float frustumHeightOrigin = 2.0f * cameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumHeightObject = 2.0f * objectDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

		float difference = frustumHeightObject - frustumHeightOrigin;
		float size = difference * (32f/360f) + 1;

		Vector3 scale = multiplier * size;
		scale.z = transform.localScale.z;

		transform.localScale = scale;
	}
}
