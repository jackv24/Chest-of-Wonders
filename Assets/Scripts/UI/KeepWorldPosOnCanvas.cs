using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepWorldPosOnCanvas : MonoBehaviour
{
    public Vector2 worldPos;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}
