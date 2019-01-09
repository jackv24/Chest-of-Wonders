using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ElementManager))]
public class ElementManagerEditor : Editor
{
    private ElementManager manager;

	public override void OnInspectorGUI()
    {
		serializedObject.Update();

        //Get selected SaveManager in inspector
        manager = (ElementManager)target;

		// Damage Values
		{
			EditorGUILayout.LabelField("Damage Multipliers", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("normalMultiplier"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ineffectiveMultiplier"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("effectiveMultiplier"));
			EditorGUILayout.Space();

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

			bool damageWrong = false;

			//Draw grid of float fields
			for (int j = 0; j < columns; j++)
			{
				EditorGUILayout.BeginHorizontal();

				//Draw element labels across side
				EditorGUILayout.LabelField(System.Enum.GetName(typeof(ElementManager.Element), j), GUILayout.Width(50));

				for (int i = 0; i < rows; i++)
				{
					EditorGUILayout.BeginVertical();

					ElementManager.Effectiveness effectiveness = manager.GetEffectiveness(i, j);

					if (i == 0 || j == 0)
					{
						if (effectiveness == ElementManager.Effectiveness.Normal)
							GUI.backgroundColor = Color.gray;
						else
						{
							GUI.backgroundColor = Color.red;
							damageWrong = true;
						}
					}
					else
					{
						//Set colour for effectiveness
						switch (effectiveness)
						{
							case ElementManager.Effectiveness.Normal:
								GUI.backgroundColor = Color.white;
								break;
							case ElementManager.Effectiveness.Effective:
								GUI.backgroundColor = Color.green;
								break;
							case ElementManager.Effectiveness.Ineffective:
								GUI.backgroundColor = Color.yellow;
								break;
						}
					}

					manager.SetEffectiveness(i, j, (ElementManager.Effectiveness)EditorGUILayout.EnumPopup(effectiveness));

					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndHorizontal();
			}

			//Reset background color for further elements
			GUI.backgroundColor = Color.white;

			if (damageWrong)
				EditorGUILayout.HelpBox("Damage changed on \"None\" column or row, please set to Normal!", MessageType.Error);
		}

		//Make sure values are saved
		if(!Application.isPlaying && GUI.changed)
        {
            GameObject obj = ((ElementManager)target).gameObject;

            //If this object is a prefab, mark it as dirty
            if (PrefabUtility.GetPrefabInstanceHandle(obj) != null)
                EditorUtility.SetDirty(target);

            EditorSceneManager.MarkSceneDirty(obj.scene);
        }

		serializedObject.ApplyModifiedProperties();
    }
}
