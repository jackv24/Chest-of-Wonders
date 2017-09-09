using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ElementManager))]
public class ElementManagerEditor : Editor
{
    ElementManager manager;

    public override void OnInspectorGUI()
    {
		serializedObject.Update();

        //Get selected SaveManager in inspector
        manager = (ElementManager)target;

		//Element Attacks
		{
			EditorGUILayout.LabelField("Base Magic Attacks", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("fireAttack"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("grassAttack"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("iceAttack"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("windAttack"));
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Attack Mix Matrix", EditorStyles.boldLabel);

			//Calculate rows and columns
			float columns = System.Enum.GetNames(typeof(ElementManager.Element)).Length;
			float rows = manager.attackArray.Length / columns;

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

				for (int i = 0; i < rows; i++)
				{
					EditorGUILayout.BeginVertical();

					MagicAttack attack = manager.GetAttack(i, j, true);

					//Set slot colours
					if (i == 0 || j == 0)
					{
						//None element should be greyed out since they aren't needed, and red if full
						if(attack)
							GUI.backgroundColor = Color.red;
						else
							GUI.backgroundColor = Color.grey;
					}
					else
					{
						//All other slots should be yellow if empty since they should be filled, and green if filled
						if(attack)
							GUI.backgroundColor = Color.green;
						else
							GUI.backgroundColor = Color.yellow;
					}

					manager.SetAttack(i, j, (MagicAttack)EditorGUILayout.ObjectField(attack, typeof(MagicAttack), false));

					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndHorizontal();
			}

			//Reset background color for further elements
			GUI.backgroundColor = Color.white;
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

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
							GUI.color = Color.gray;
						else
							GUI.color = Color.red;
					}
					else
					{
						//Set colour for effectiveness
						switch (effectiveness)
						{
							case ElementManager.Effectiveness.Normal:
								GUI.color = Color.white;
								break;
							case ElementManager.Effectiveness.Effective:
								GUI.color = Color.green;
								break;
							case ElementManager.Effectiveness.Ineffective:
								GUI.color = Color.yellow;
								break;
						}
					}

					manager.SetEffectiveness(i, j, (ElementManager.Effectiveness)EditorGUILayout.EnumPopup(effectiveness));

					EditorGUILayout.EndVertical();
				}

				//Reset background color for further elements
				GUI.backgroundColor = Color.white;

				EditorGUILayout.EndHorizontal();
			}
		}

		//Make sure values are saved
		if(!Application.isPlaying && GUI.changed)
        {
            GameObject obj = ((ElementManager)target).gameObject;

            //If this object is a prefab, mark it as dirty
            if (PrefabUtility.GetPrefabObject(obj) != null)
                EditorUtility.SetDirty(target);

            EditorSceneManager.MarkSceneDirty(obj.scene);
        }

		serializedObject.ApplyModifiedProperties();
    }
}
