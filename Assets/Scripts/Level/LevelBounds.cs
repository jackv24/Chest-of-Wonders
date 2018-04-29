using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class LevelBounds : MonoBehaviour
{
    public Vector2 centre = Vector2.zero;

    public float width = 10f;
    public float height = 10f;

	private void Reset()
	{
		STETilemap[] tilemaps = GetComponentsInChildren<STETilemap>();

		if (tilemaps.Length > 0)
		{
			float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;

			foreach (STETilemap tileMap in tilemaps)
			{
				Bounds b = tileMap.MapBounds;

				minX = Mathf.Min(minX, b.min.x);
				maxX = Mathf.Max(maxX, b.max.x);
				minY = Mathf.Min(minY, b.min.y);
				maxY = Mathf.Max(maxY, b.max.y);
			}

			Bounds bounds = new Bounds();
			bounds.SetMinMax(new Vector3(minX, minY), new Vector3(maxX, maxY));

			centre = bounds.center;
			width = bounds.size.x;
			height = bounds.size.y;
		}
	}

	private void Start()
    {
		if(CameraControl.Instance)
			CameraControl.Instance.SetBounds(this);
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
