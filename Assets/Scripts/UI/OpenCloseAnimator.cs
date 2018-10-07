using UnityEngine;
using System;

[Serializable]
public class OpenCloseAnimator
{
	public Animator Animator;
	public string OpenAnim = "Open";
	public string CloseAnim = "Close";
	public bool areMirrored = true;

	private bool isOpen;

	public bool IsOpen
	{
		get
		{
			if (!Animator)
				return isOpen;

			if (isOpen)
				return true;

			var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
			return stateInfo.normalizedTime < stateInfo.length;
		}
	}

	public void PreClose()
	{
		if (!Animator)
			return;

		Animator.Play(CloseAnim, 0, 1.0f);
		isOpen = false;
	}

	public void PlayOpen()
	{
		if (!Animator)
			return;

		Animator.Play(OpenAnim);

		isOpen = true;
	}

	public void PlayClose()
	{
		if (!Animator)
			return;

		if (areMirrored)
			Animator.Play(CloseAnim, 0, 1 - Mathf.Clamp01(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
		else
			Animator.Play(CloseAnim);

		isOpen = false;
	}
}
