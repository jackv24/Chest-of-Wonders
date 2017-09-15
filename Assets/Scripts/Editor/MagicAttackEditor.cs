using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MagicAttack))]
public class MagicAttackEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("manaCost"));

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));

		EditorGUILayout.Space();
		switch(((MagicAttack)target).type)
		{
			case MagicAttack.Type.Projectile:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("castEffect"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("projectilePrefab"));
				break;
			case MagicAttack.Type.Animation:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("animation"));
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
