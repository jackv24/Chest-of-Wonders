using UnityEngine;
using System.Collections;

public static class Helper
{
    public static Vector3 SnapTo(Vector3 vector, float snapAngle)
    {
        float angle = Vector3.Angle(vector, Vector3.up);

        if (angle < snapAngle / 2.0f)          // Cannot do cross product 
            return Vector3.up * vector.magnitude;  //   with angles 0 & 180
        if (angle > 180.0f - snapAngle / 2.0f)
            return Vector3.down * vector.magnitude;

        float t = Mathf.Round(angle / snapAngle);
        float deltaAngle = (t * snapAngle) - angle;

        Vector3 axis = Vector3.Cross(Vector3.up, vector);

        Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);

        return q * vector;
    }

    public static void SetRotationZ(this Transform transform, float rotationZ)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z = rotationZ;
        transform.eulerAngles = eulerAngles;
    }
}
