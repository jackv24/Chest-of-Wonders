using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockArea : MonoBehaviour
{
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	public Bounds Bounds
	{
		get
		{
			Bounds bounds = new Bounds();

			bounds.min = new Vector3(minX, minY, 0);
			bounds.max = new Vector3(maxX, maxY, 0);

			return bounds;
		}
	}

	private int insideCount = 0;

	private void Reset()
	{
		minX = transform.position.x - 2;
		maxX = transform.position.x + 2;
		minY = transform.position.y - 2;
		maxY = transform.position.y + 2;
	}

	private void OnDrawGizmosSelected()
	{
		Bounds bounds = Bounds;

		Gizmos.color = new Color(0, 1, 0, 0.25f);
		Gizmos.DrawWireCube(bounds.center, bounds.size);

		Gizmos.color = new Color(0, 1, 0, 0.1f);
		Gizmos.DrawCube(bounds.center, bounds.size);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		insideCount++;
		if (insideCount > 1)
			return;

		if (CameraControl.Instance)
			CameraControl.Instance.AddCameraLock(this);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		insideCount--;
		if (insideCount > 0)
			return;

		if (CameraControl.Instance)
			CameraControl.Instance.RemoveCameraLock(this);
	}
}
