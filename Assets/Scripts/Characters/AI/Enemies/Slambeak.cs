using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Slambeak : MonoBehaviour
{
	public float attackRange = 4.0f;

	public bool FacingRight { get { return transform.localScale.x < 0 ? true : false; } }

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

	private Transform player;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		GameObject obj = GameObject.FindWithTag("Player");

		if (obj)
			player = obj.transform;

		//Only react to player if there is a player found
		if (player)
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
			Vector2 offset = player.transform.position - transform.position;

			//Only attack if in range and in facing direction
			if(offset.magnitude <= attackRange && ((FacingRight && offset.x > 0) || (!FacingRight && offset.x < 0)))
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

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + Vector3.up * attackRange);
		Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + (FacingRight ? Vector3.right : Vector3.left) * attackRange);

		Handles.color = Color.red;
		Handles.DrawWireArc(transform.position, -Vector3.forward, (FacingRight ? Vector3.right : Vector3.left), (FacingRight ? -90 : 90), attackRange);
		Handles.color = new Color(1, 0, 0, 0.1f);
		Handles.DrawSolidArc(transform.position, -Vector3.forward, (FacingRight ? Vector3.right : Vector3.left), (FacingRight ? -90 : 90), attackRange);
	}
#endif
}
