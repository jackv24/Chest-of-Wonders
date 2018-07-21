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
		var a = Camera.main.WorldToScreenPoint(transform.position);
		var b = new Vector3(a.x, a.y + pixelsPerUnit, a.z);

		var aa = Camera.main.ScreenToWorldPoint(a);
		var bb = Camera.main.ScreenToWorldPoint(b);

		transform.localScale = multiplier * (aa - bb).magnitude;
	}
}
