using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepWorldPosOnCanvas : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnGetWorldPos;

    public Vector2 worldPos;

    public bool keepInScreenBounds = false;

    public float paddingTop;
    public float paddingDown;
    public float paddingLeft;
    public float paddingRight;

    private Vector2 screenMax;
    private Vector2 screenMin;

	private Camera gameCamera;
	private Camera uiCamera;

	private void Start()
	{
		gameCamera = CameraManager.GameCamera;
		uiCamera = CameraManager.UICamera;

		Canvas.willRenderCanvases += UpdatePos;
	}

	private void OnDestroy()
	{
		Canvas.willRenderCanvases -= UpdatePos;
	}

	private void UpdatePos()
	{
		if (!enabled || !gameObject.activeInHierarchy)
			return;

		GetWorldPos();

		//UICamera should be at world origin, so offset from game camera should be canvas position
		Vector2 pos = worldPos - (Vector2)gameCamera.transform.position;

		if (keepInScreenBounds)
		{
			screenMax = uiCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
			screenMin = uiCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));

			pos.x = Mathf.Clamp(pos.x, screenMin.x + paddingLeft, screenMax.x - paddingRight);
			pos.y = Mathf.Clamp(pos.y, screenMin.y + paddingDown, screenMax.y - paddingTop);

		}

		transform.position = pos.SnapToGrid();//screenPos;
	}

	public void GetWorldPos()
	{
		if (OnGetWorldPos != null)
			OnGetWorldPos();
	}
}
