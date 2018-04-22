using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraLockArea : MonoBehaviour
{
	public float paddingLeft = 2.0f;
	public float paddingRight = 2.0f;
	public float paddingTop = 2.0f;
	public float paddingBottom = 2.0f;

	public Bounds Bounds
	{
		get
		{
			Bounds bounds = box.bounds;

			bounds.min = new Vector3(bounds.min.x - paddingLeft, bounds.min.y - paddingBottom, bounds.min.z);
			bounds.max = new Vector3(bounds.max.x + paddingRight, bounds.max.y + paddingTop, bounds.max.z);

			return bounds;
		}
	}

	private BoxCollider2D box;

	private void Awake()
	{
		box = GetComponent<BoxCollider2D>();
	}

	private void OnDrawGizmosSelected()
	{
		if (!box)
			box = GetComponent<BoxCollider2D>();

		if(box)
		{
			Bounds bounds = Bounds;

			Gizmos.color = new Color(0, 1, 0, 0.25f);
			Gizmos.DrawWireCube(bounds.center, bounds.size);

			Gizmos.color = new Color(0, 1, 0, 0.1f);
			Gizmos.DrawCube(bounds.center, bounds.size);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CameraControl.Instance)
			CameraControl.Instance.AddCameraLock(this);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (CameraControl.Instance)
			CameraControl.Instance.RemoveCameraLock(this);
	}
}
