using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
	public AnimationClip disappearAnimation;
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void ShowPrompt(InteractType interactType)
	{
        if (animator)
            animator.SetBool("visible", true);
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
