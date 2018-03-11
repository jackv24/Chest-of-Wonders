using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraLockArea : MonoBehaviour
{
	public float paddingX = 2.0f;
	public float paddingY = 2.0f;

	public Bounds Bounds { get{ return box.bounds; } }

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
			Gizmos.color = new Color(0, 1, 0, 0.25f);
			Gizmos.DrawWireCube((Vector2)transform.position + box.offset, box.size + new Vector2(paddingX * 2, paddingY * 2));

			Gizmos.color = new Color(0, 1, 0, 0.1f);
			Gizmos.DrawCube((Vector2)transform.position + box.offset, box.size + new Vector2(paddingX * 2, paddingY * 2));
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CameraFollow.Instance)
			CameraFollow.Instance.AddCameraLock(this);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (CameraFollow.Instance)
			CameraFollow.Instance.RemoveCameraLock(this);
	}
}
