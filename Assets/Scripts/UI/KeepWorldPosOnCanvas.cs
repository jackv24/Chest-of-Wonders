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

	private Camera screenCam;

	private void Start()
	{
		GameObject cam = GameObject.FindWithTag("ScreenCamera");
		if (cam)
			screenCam = cam.GetComponent<Camera>();
	}

	private void LateUpdate()
    {
		GetWorldPos();

		Vector3 position;

        if(keepInScreenBounds)
        {
            Vector2 pos = worldPos;

            screenMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
            screenMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

            pos.x = Mathf.Clamp(pos.x, screenMin.x + paddingLeft, screenMax.x - paddingRight);
            pos.y = Mathf.Clamp(pos.y, screenMin.y + paddingDown, screenMax.y - paddingTop);

            position = pos;
        }
        else
            position = worldPos;

		///Convert to screen coordinates (since we're using a second camera to render the game)
		//First get viewport position for the main camera...
		Vector3 mainPos = Camera.main.WorldToViewportPoint(position);
		//...then convert this viewport point to screen point on the screen camera (since their viewports should be the same)
		Vector3 screenPos = screenCam.ViewportToScreenPoint(mainPos);

		transform.position = screenPos;
    }

	public void GetWorldPos()
	{
		if (OnGetWorldPos != null)
			OnGetWorldPos();
	}
}
