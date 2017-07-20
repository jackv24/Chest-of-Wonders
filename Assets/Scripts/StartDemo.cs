using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartDemo : MonoBehaviour
{
	public float fadeTime = 2.0f;

	private CanvasRenderer[] renderers;

	public Button firstButton;

	void Start()
	{
		renderers = GetComponentsInChildren<CanvasRenderer>();

		GameManager.instance.gamePaused = true;

		if (firstButton)
			EventSystem.current.firstSelectedGameObject = firstButton.gameObject;
	}

	public void FadeStart()
	{
		StartCoroutine("FadeOut");
	}

	IEnumerator FadeOut()
	{
		float elapsedTime = 0;

		while (elapsedTime <= fadeTime)
		{
			float opacity = 1 - (elapsedTime / fadeTime);

			foreach (CanvasRenderer rend in renderers)
			{
				rend.SetAlpha(opacity);
			}

			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		
		GameManager.instance.gamePaused = false;

		gameObject.SetActive(false);
	}
}
