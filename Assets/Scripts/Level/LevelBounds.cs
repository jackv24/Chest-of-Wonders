using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour
{
    public Vector2 centre = Vector2.zero;

    public float width = 10f;
    public float height = 10f;

    private void Start()
    {
        CameraFollow cam = FindObjectOfType<CameraFollow>();

        if(cam)
            cam.SetBounds(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(centre, new Vector3(width, height, 1));

        Gizmos.color = new Color(1, 0, 0, 0.25f);

        float edgeWidth = 5f;

        Gizmos.DrawCube(new Vector3(centre.x - width / 2 - edgeWidth / 2, centre.y, 1), new Vector3(edgeWidth, height, 1));
        Gizmos.DrawCube(new Vector3(centre.x + width / 2 + edgeWidth / 2, centre.y, 1), new Vector3(edgeWidth, height, 1));

        Gizmos.DrawCube(new Vector3(centre.x, centre.y - height / 2 - edgeWidth / 2, 1), new Vector3(width + edgeWidth * 2, edgeWidth, 1));
        Gizmos.DrawCube(new Vector3(centre.x, centre.y + height / 2 + edgeWidth / 2, 1), new Vector3(width + edgeWidth * 2, edgeWidth, 1));
    }
}
