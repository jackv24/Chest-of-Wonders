using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
	public TextMeshProUGUI interactText;

	[Space()]
	public string openText = "UI/INTERACT_OPEN";
	public string talkText = "UI/INTERACT_TALK";

	[Space()]
	public AnimationClip disappearAnimation;
	private Animator animator;

	private void Reset()
	{
		interactText = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void ShowPrompt(InteractType interactType)
	{
		if(interactText)
		{
			string text = "Interact";

			switch(interactType)
			{
				case InteractType.Open:
					text = openText;
					break;
				case InteractType.Speak:
					text = talkText;
					break;
			}

			interactText.text = text.TryGetTranslation();

			if (animator)
				animator.SetBool("visible", true);
		}
	}

	public void HidePrompt()
	{
		StartCoroutine(Disappear());
	}

	IEnumerator Disappear()
	{
		if (animator)
			animator.SetBool("visible", false);

		if (disappearAnimation)
			yield return new WaitForSeconds(disappearAnimation.length);

		gameObject.SetActive(false);
	}
}
