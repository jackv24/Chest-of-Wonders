using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.Integer)
		{
			property.intValue = EditorGUI.LayerField(position, label, property.intValue);
		}
		else
			base.OnGUI(position, property, label);
	}
}
