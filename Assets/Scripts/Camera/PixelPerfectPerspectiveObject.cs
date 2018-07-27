using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectPerspectiveObject : MonoBehaviour
{
	public const float PixelsPerUnit = PixelPerfectPerspectiveCamera.PixelsPerUnit;

	public Vector2 Multiplier = Vector2.one;

	public bool UpdateEveryFrame;

	private void Start()
	{
		UpdateSize();
	}

	private void Update()
	{
		if(!Application.isPlaying || UpdateEveryFrame)
		{
			UpdateSize();
		}
	}

	private void UpdateSize()
	{
		Camera cam = Camera.main;

		if (!cam) return;
		
		float cameraDistance = -cam.transform.position.z;
		float objectDistance = transform.position.z - cam.transform.position.z;

		float frustumHeightOrigin = 2.0f * cameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumHeightObject = 2.0f * objectDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

		float difference = frustumHeightObject - frustumHeightOrigin;
		float size = difference * (32f/360f) + 1;

		Vector3 scale = Multiplier * size;
		scale.z = transform.localScale.z;

		transform.localScale = scale;
	}
}
