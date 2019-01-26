using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Slambeak : MonoBehaviour
{
	public TriggerDetector slamTrigger;

	[Space()]
	public float stunTime = 2.0f;
	public float recoverTime = 1.0f;
	public float attackPauseTime = 1.0f;

	[Header("Animations")]
	public AnimationClip idleAnim;
	public AnimationClip attackDownAnim;
	public AnimationClip stunAnim;
	public AnimationClip recoverAnim;
	public AnimationClip attackUpAnim;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		if(slamTrigger)
			StartCoroutine(Behaviour());
	}

	void PlayAnim(AnimationClip clip)
	{
		if(animator)
		{
			animator.Play(clip.name);
		}
	}

	IEnumerator Behaviour()
	{
		//Behaviour loops while this gameobject is active
		while(true)
		{
			if(slamTrigger.InsideCount > 0)
			{
				//Attack is it's own routine
				yield return StartCoroutine(Attack());

				//After attack, return to idle and wait
				PlayAnim(idleAnim);
				yield return new WaitForSeconds(attackPauseTime);
			}

			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Attack()
	{
		//Play attack sequence
		PlayAnim(attackDownAnim);
		yield return new WaitForSeconds(attackDownAnim.length);

		PlayAnim(stunAnim);
		yield return new WaitForSeconds(stunTime);

		PlayAnim(recoverAnim);
		yield return new WaitForSeconds(recoverTime);

		PlayAnim(attackUpAnim);
		yield return new WaitForSeconds(attackUpAnim.length);
	}
}
