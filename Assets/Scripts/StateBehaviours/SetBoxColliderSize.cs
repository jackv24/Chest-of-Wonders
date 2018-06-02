using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoxColliderSize : CharacterStateBehaviour
{
	public Vector2 offset;
	public Vector2 size;

	private Vector2 oldOffset;
	private Vector2 oldSize;

	private BoxCollider2D collider;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		collider = GetComponentAtLevel<BoxCollider2D>(animator.gameObject);

		if(collider)
		{
			//Cache old values before setting new ones to be reset on exit
			oldOffset = collider.offset;
			oldSize = collider.size;

			collider.offset = offset;
			collider.size = size;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(collider)
		{
			//Reset previous values on exit
			collider.offset = oldOffset;
			collider.size = oldSize;
		}
	}
}
