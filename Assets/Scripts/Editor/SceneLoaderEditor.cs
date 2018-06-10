using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneLoaderEditor : EditorWindow
{
	private Dictionary<string, Color> sceneButtonColours = new Dictionary<string, Color>()
	{
		{"Demo", new Color(0.5f, 0.5f, 1.0f)},
		{"Fortress", Helper.RGBToColor(233, 210, 19)}
	};

	private Vector2 scrollPos = Vector2.zero;

    private bool autoAddGameScene = true;

	[MenuItem("Overgrowth Tools/Scene Loader")]
	static void Init()
	{
		SceneLoaderEditor window = (SceneLoaderEditor)EditorWindow.GetWindow(typeof(SceneLoaderEditor));

		window.titleContent = new GUIContent("Scene Loader");

		window.Show();
	}

	private void OnGUI()
	{
		Color backColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.cyan;
		if (GUILayout.Button("Open Game Scene", GUILayout.Height(30)))
		{
			EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(1), OpenSceneMode.Single);
		}
		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Add Game Scene", GUILayout.Height(30)))
		{
            AddGameScene();
		}
		GUI.backgroundColor = Color.yellow;
		if (GUILayout.Button("Close Game Scene", GUILayout.Height(30)))
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByBuildIndex(1), true);
		}
		EditorGUILayout.EndHorizontal();

		GUI.backgroundColor = backColor;

        EditorGUILayout.Space();
        autoAddGameScene = EditorGUILayout.Toggle("Auto-add Game scene", autoAddGameScene);

		EditorGUILayout.Space();
		GUILayout.Label("Scenes in build", EditorStyles.boldLabel);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		int columnCount = 0;
		EditorGUILayout.BeginHorizontal();
		for(int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

			//Keep track of how many columns
			columnCount++;

			//Set button colour
			GUI.backgroundColor = GetColorForSceneName(sceneName);

			//Buttons to load this scene
			if(GUILayout.Button(sceneName, GUILayout.Height(30), GUILayout.Width(position.width / 2 - 13)))
			{
                //Only load scene if user confirms edited scene
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);

                    if (autoAddGameScene)
                        AddGameScene(scenePath);
                }
			}

			//Reset columns once target reached
			if(columnCount >= 2)
			{
				columnCount = 0;

				//Move layout to next line
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();

		GUI.backgroundColor = backColor;
	}

    void AddGameScene(string firstScene = "")
    {
        if (firstScene.Contains("Game") || firstScene.Contains("MainMenu"))
            return;

        Scene scene = EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(1), OpenSceneMode.Additive);
        Scene beforeScene = EditorSceneManager.GetActiveScene();

        EditorSceneManager.SetActiveScene(beforeScene);

        EditorSceneManager.MoveSceneBefore(scene, beforeScene);
    }

	Color GetColorForSceneName(string sceneName)
	{
		string[] splitStrings = sceneName.Split('_');

		if(splitStrings.Length > 0)
		{
			//First string in split is the prefix
			string prefix = splitStrings[0];

			if(sceneButtonColours.ContainsKey(prefix))
				return sceneButtonColours[prefix];
		}

		return Color.white;
	}
}
