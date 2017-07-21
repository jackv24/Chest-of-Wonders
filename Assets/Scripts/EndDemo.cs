using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDemo : MonoBehaviour
{
	public float endDelay = 5.0f;

	public float fadeTime = 1.0f;

	private CanvasRenderer[] renderers;

	private void Start()
	{
		GameManager.instance.endDemo = this;

		renderers = GetComponentsInChildren<CanvasRenderer>();

		gameObject.SetActive(false);
	}

	public void End()
	{
		foreach (CanvasRenderer rend in renderers)
		{
			rend.SetAlpha(0);
		}

		gameObject.SetActive(true);

		StartCoroutine("FadeIn");
	}

	IEnumerator FadeIn()
	{
		yield return new WaitForSeconds(endDelay);

		CharacterMove playerMove = GameManager.instance.player.GetComponent<CharacterMove>();

		if (playerMove)
			playerMove.Move(0);

		GameManager.instance.gameRunning = false;

		float elapsedTime = 0;

		while (elapsedTime <= fadeTime)
		{
			float opacity = elapsedTime / fadeTime;

			foreach (CanvasRenderer rend in renderers)
			{
				rend.SetAlpha(opacity);
			}

			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}

		foreach (CanvasRenderer rend in renderers)
		{
			rend.SetAlpha(1);
		}
	}
}
