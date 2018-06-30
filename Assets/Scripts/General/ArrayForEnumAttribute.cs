using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArrayForEnumAttribute : PropertyAttribute
{
	public Type enumType;

	public ArrayForEnumAttribute(Type enumType)
	{
		this.enumType = enumType;
	}

	public static void EnsureArraySize<T>(ref T[] array, Type enumType)
	{
		int size = Enum.GetNames(enumType).Length;
		if(array.Length != size)
		{
			T[] oldArray = array;
			array = new T[size];

			for(int i = 0; i < Mathf.Min(size, oldArray.Length); i++)
			{
				array[i] = oldArray[i];
			}
		}
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ArrayForEnumAttribute))]
public class ArrayForEnumDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		try
		{
			var config = attribute as ArrayForEnumAttribute;
			string[] enumNames = Enum.GetNames(config.enumType);

			int index = int.Parse(property.propertyPath.Split('[', ']')[1]);

			string enumLabel = ObjectNames.NicifyVariableName(enumNames[index]);
			label = new GUIContent(enumLabel);
		}
		catch
		{

		}

		EditorGUI.PropertyField(position, property, label, property.isExpanded);
	}
}
#endif
