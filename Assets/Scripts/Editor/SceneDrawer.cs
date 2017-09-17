using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.Integer)
		{
			GUIContent[] sceneNames = new GUIContent[EditorSceneManager.sceneCountInBuildSettings];
			int[] indexes = new int[EditorSceneManager.sceneCountInBuildSettings];

			for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
			{
				string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
				sceneNames[i] = new GUIContent(System.IO.Path.GetFileNameWithoutExtension(scenePath));

				indexes[i] = i;
			}

			EditorGUI.IntPopup(position, property, sceneNames, indexes);
		}
		else
			base.OnGUI(position, property, label);
	}
}
