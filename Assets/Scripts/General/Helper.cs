using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

[System.Serializable]
public struct MinMaxFloat
{
	public float min;
	public float max;

	public float RandomValue { get { return Random.Range(min, max); } }

	public MinMaxFloat(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMaxFloat))]
public class MinMaxFloatDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		float width = position.width / 2;
		Rect minRect = new Rect(position.x, position.y, width, position.height);
		Rect maxRect = new Rect(position.x + width, position.y, width, position.height);

		EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
		EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);

		EditorGUI.EndProperty();
	}
}
#endif
