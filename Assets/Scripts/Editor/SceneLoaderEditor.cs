using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneLoaderEditor : EditorWindow
{
	private string saveLocation;

	private Color currentColor = Color.white;

	private Vector2 scrollPos = Vector2.zero;

	[System.Serializable]
	class GameScene
	{
		public int buildIndex = -1;
		public string sceneName = "Scene";

		public Color buttonColor;

		public GameScene()
		{

		}

		public GameScene(int buildIndex, string sceneName, Color buttonColor)
		{
			this.buildIndex = buildIndex;
			this.sceneName = sceneName;
			this.buttonColor = buttonColor;
		}
	}

	[System.Serializable]
	class SaveData
	{
		public List<GameScene> scenes = new List<GameScene>();
	}

	private SaveData data;

	[MenuItem("Overgrowth Tools/Scene Loader")]
	static void Init()
	{
		SceneLoaderEditor window = (SceneLoaderEditor)EditorWindow.GetWindow(typeof(SceneLoaderEditor));

		window.titleContent = new GUIContent("Scene Loader");

		window.Show();
	}

	private void OnEnable()
	{
		//Create string for save location
		saveLocation = Application.dataPath + "/SceneLoader.json";

		//Try and load save data, else create a new empty one
		if (!Load())
		{
			data = new SaveData();
		}
	}

	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		//Color picker for added scene
		currentColor = EditorGUILayout.ColorField(currentColor);

		if(GUILayout.Button("Add Current"))
		{
			Scene scene = SceneManager.GetActiveScene();

			int existingIndex = -1;

			//See if this scene is already added
			foreach (GameScene s in data.scenes)
			{
				if (s.buildIndex == scene.buildIndex)
					existingIndex = data.scenes.IndexOf(s);
			}

			GameScene gameScene = new GameScene();

			//Save scene data into game scene
			gameScene.buildIndex = scene.buildIndex;
			gameScene.sceneName = scene.name;
			gameScene.buttonColor = currentColor;

			//Update or add game scene
			if (existingIndex >= 0)
				data.scenes[existingIndex] = gameScene;
			else
				data.scenes.Add(gameScene);

			//Save data to editor file
			Save();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		GUILayout.Label("Load Scene", EditorStyles.boldLabel);

		Color backColor = GUI.backgroundColor;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		int columnCount = 0;
		EditorGUILayout.BeginHorizontal();
		foreach(GameScene scene in data.scenes)
		{
			//Keep track of how many columns
			columnCount++;

			GUI.backgroundColor = scene.buttonColor;

			//Buttons to load this scene
			if(GUILayout.Button(scene.sceneName, GUILayout.Height(30), GUILayout.Width(position.width / 2 - 10)))
			{
				//Only load scene if user confirms edited scene
				if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(scene.buildIndex));
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

	private void Save()
	{
		string json = JsonUtility.ToJson(data, true);

		System.IO.File.WriteAllText(saveLocation, json);
	}

	private bool Load()
	{
		if (System.IO.File.Exists(saveLocation))
		{
			string json = System.IO.File.ReadAllText(saveLocation);

			data = (SaveData)JsonUtility.FromJson(json, typeof(SaveData));

			return true;
		}
		else
			return false;
	}
}
