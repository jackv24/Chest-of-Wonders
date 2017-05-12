using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElementManager))]
public class ElementManagerEditor : Editor
{
    ElementManager manager;

    public override void OnInspectorGUI()
    {
        //Get selected SaveManager in inspector
        manager = (ElementManager)target;

        EditorGUILayout.LabelField("Damage Matrix", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Matrix is Source vs. Target.\nAttack element is rows, defense element is columns", MessageType.Info);

        //Calculate rows and columns
        float columns = System.Enum.GetNames(typeof(ElementManager.Element)).Length;
        float rows = manager.damageArray.Length / columns;

        EditorGUILayout.BeginHorizontal();
            //Draw blank space at start for indentation
            EditorGUILayout.LabelField("", GUILayout.Width(50));

            //Draw element labels across top
            for (int i = 0; i < columns; i++)
            {
                EditorGUILayout.LabelField(System.Enum.GetName(typeof(ElementManager.Element), i), GUILayout.MinWidth(0));
            }
        EditorGUILayout.EndHorizontal();

        //Draw grid of float fields
        for (int j = 0; j < columns; j++)
        {
            EditorGUILayout.BeginHorizontal();

            //Draw element labels across side
            EditorGUILayout.LabelField(System.Enum.GetName(typeof(ElementManager.Element), j), GUILayout.Width(50));

            for(int i = 0; i < rows; i++)
            {
                EditorGUILayout.BeginVertical();

                manager.SetDamageValue(i, j, EditorGUILayout.FloatField(manager.GetDamageValue(i, j)));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }

        //Make sure values are saved
        EditorUtility.SetDirty(target);
    }
}
