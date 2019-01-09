using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindMissingScripts
{
	[MenuItem("Custom Tools/Remove Missing Scripts in Scenes")]
	public static void FindMissingComponentsInScenes()
	{
		bool wasFound = false;

		for(int i = 0; i < SceneManager.sceneCount; i++)
		{
			GameObject[] objs = SceneManager.GetSceneAt(i).GetRootGameObjects();

			foreach (var obj in objs)
			{
				FindMissingComponentsInGameObject(obj, true, ref wasFound);
			}
		}

		if (!wasFound)
			Debug.Log("No missing components were found in any open scenes!");
	}

	[MenuItem("Custom Tools/Remove Missing Scripts in Project")]
	public static void FindMissingComponentsInProject()
	{
		if (!EditorUtility.DisplayDialog("Are you sure?", "This may take a while, and will touch every GameObject in the project!", "YES", "NO"))
			return;

		bool wasFound = false;

		string[] guids = AssetDatabase.FindAssets("t:GameObject");
		foreach(var guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			if (obj)
				FindMissingComponentsInGameObject(obj, true, ref wasFound);
		}

		if (!wasFound)
			Debug.Log("No missing components were found on GameObjects in project!");
	}

	private static void FindMissingComponentsInGameObject(GameObject obj, bool recursive, ref bool wasFound)
	{
		int removedCount = 0;
		Component[] components = obj.GetComponents<Component>();
		for(int i = 0; i < components.Length; i++)
		{
			if(components[i] == null)
			{
				wasFound = true;

				// Remove null component
				var serializedObject = new SerializedObject(obj);
				var property = serializedObject.FindProperty("m_Component");
				property.DeleteArrayElementAtIndex(i - removedCount);
				removedCount++;
				serializedObject.ApplyModifiedProperties();

				// Get GameObject hierarchy path by looping up through parents
				string path = obj.name;
				Transform t = obj.transform;
				while(t.parent != null)
				{
					path = $"{t.parent.name}/{path}";
					t = t.parent;
				}

				Debug.Log($"Found missing component: \"{path}\"");
			}
		}

		if (recursive)
		{
			foreach (Transform child in obj.transform)
				FindMissingComponentsInGameObject(child.gameObject, true, ref wasFound);
		}
	}
}
