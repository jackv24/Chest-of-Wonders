using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisabled : MonoBehaviour
{
	public GameObject[] gameObjects;

	private void Start()
	{
		StartCoroutine(EnableGameObjects());
	}

	IEnumerator EnableGameObjects()
	{
		List<GameObject> disable = new List<GameObject>(gameObjects.Length);

		foreach (GameObject obj in gameObjects)
		{
			if (!obj.activeSelf)
			{
				obj.SetActive(true);

				disable.Add(obj);
			}
		}

		yield return null;

		foreach(GameObject obj in disable)
		{
			obj.SetActive(false);
		}
	}
}
