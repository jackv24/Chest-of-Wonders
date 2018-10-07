using UnityEngine;
using System;

[Serializable]
public class OpenCloseAnimator
{
	public Animator Animator;
	public string OpenAnim = "Open";
	public string CloseAnim = "Close";
	public bool areMirrored = true;

	public void PreClose()
	{
		if (!Animator)
			return;

		Animator.Play(CloseAnim, 0, 1.0f);
	}

	public void PlayOpen()
	{
		if (!Animator)
			return;

		Animator.Play(OpenAnim);
	}

	public void PlayClose()
	{
		if (!Animator)
			return;

		if (areMirrored)
			Animator.Play(CloseAnim, 0, 1 - Mathf.Clamp01(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
		else
			Animator.Play(CloseAnim);
	}
}
