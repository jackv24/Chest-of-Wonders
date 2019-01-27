using UnityEngine;

public static class CustomGizmos
{
    private const float POINT_SPHERE_RADIUS = 0.1f;

    public static void DrawPoint(Vector3 point)
    {
        Gizmos.DrawWireSphere(point, POINT_SPHERE_RADIUS);
    }
}
