using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindMissingScripts
{
	[MenuItem("Overgrowth Tools/Find Missing Scripts in Scene")]
	public static void FindMissingComponents()
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

	private static void FindMissingComponentsInGameObject(GameObject obj, bool recursive, ref bool wasFound)
	{
		Component[] components = obj.GetComponents<Component>();
		for(int i = 0; i < components.Length; i++)
		{
			if(components[i] == null)
			{
				wasFound = true;

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
