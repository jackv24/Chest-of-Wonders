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

	private Camera uiCamera;

	private void Start()
	{
		GameObject cam = GameObject.FindWithTag("UICamera");
		if (cam)
			uiCamera = cam.GetComponent<Camera>();

		Canvas.willRenderCanvases += UpdatePos;
	}

	private void OnDestroy()
	{
		Canvas.willRenderCanvases -= UpdatePos;
	}

	private void UpdatePos()
	{
		GetWorldPos();

		Vector3 position;

		if (keepInScreenBounds)
		{
			Vector2 pos = worldPos;

			screenMax = uiCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
			screenMin = uiCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));

			pos.x = Mathf.Clamp(pos.x, screenMin.x + paddingLeft, screenMax.x - paddingRight);
			pos.y = Mathf.Clamp(pos.y, screenMin.y + paddingDown, screenMax.y - paddingTop);

			position = pos;
		}
		else
			position = worldPos;

		transform.position = position.SnapToGrid();//screenPos;
	}

	public void GetWorldPos()
	{
		if (OnGetWorldPos != null)
			OnGetWorldPos();
	}
}
