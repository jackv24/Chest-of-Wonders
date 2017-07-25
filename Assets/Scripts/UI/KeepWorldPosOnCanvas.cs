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

    private void Update()
    {
		GetWorldPos();

        if(keepInScreenBounds)
        {
            Vector2 pos = worldPos;

            screenMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
            screenMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

            pos.x = Mathf.Clamp(pos.x, screenMin.x + paddingLeft, screenMax.x - paddingRight);
            pos.y = Mathf.Clamp(pos.y, screenMin.y + paddingDown, screenMax.y - paddingTop);

            transform.position = Camera.main.WorldToScreenPoint(pos);
        }
        else
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

	public void GetWorldPos()
	{
		if (OnGetWorldPos != null)
			OnGetWorldPos();
	}
}
