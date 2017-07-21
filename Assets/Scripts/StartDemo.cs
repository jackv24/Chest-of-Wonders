using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartDemo : MonoBehaviour
{
	public float fadeTime = 2.0f;

	private CanvasRenderer[] renderers;

	public Button firstButton;

	[Space()]
	public SceneField awakeScene;
	public AnimationClip awakeAnim;

	private CharacterAnimator playerAnim;
	private CharacterMove playerMove;
	private bool asleep = false;

	void Start()
	{
		renderers = GetComponentsInChildren<CanvasRenderer>();

		GameManager.instance.gamePaused = true;

		if (firstButton)
			EventSystem.current.firstSelectedGameObject = firstButton.gameObject;

		playerAnim = GameManager.instance.player.GetComponent<CharacterAnimator>();
		playerMove = GameManager.instance.player.GetComponent<CharacterMove>();

		SetAsleep();
	}

	public void SetAsleep()
	{
		bool found = false;

		for(int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == awakeScene.SceneName)
				found = true;
		}

		if (found && playerAnim)
			playerAnim.animator.SetBool("asleep", true);

		asleep = found;
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
		
		playerAnim.animator.SetBool("asleep", false);

		if(asleep && awakeAnim)
			yield return new WaitForSeconds(awakeAnim.length);

		GameManager.instance.gamePaused = false;

		if (asleep && playerMove)
		{
			playerMove.Move(-1);
			playerMove.Move(0);
		}

		asleep = false;

		gameObject.SetActive(false);
	}
}
