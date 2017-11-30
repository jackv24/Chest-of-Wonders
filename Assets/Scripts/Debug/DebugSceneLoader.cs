using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugSceneLoader : MonoBehaviour
{
	private InputField inputField;

	private bool loading = false;

	private void Start()
	{
		if (!Debug.isDebugBuild)
			gameObject.SetActive(false);

		inputField = GetComponent<InputField>();

		if(inputField)
		{
			inputField.onEndEdit.AddListener((string input) =>
			{
				if (SceneUtility.GetBuildIndexByScenePath(input) > 1 && !loading)
				{
					loading = true;

					SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
					SceneManager.LoadSceneAsync(input, LoadSceneMode.Additive);
				}
			});
		}
	}
}
