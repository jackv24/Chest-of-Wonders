using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoxColliderSize : StateMachineBehaviour
{
	public enum GetLevel
	{
		Self, Parents, Children
	}
	public GetLevel getLevel;

	public Vector2 offset;
	public Vector2 size;

	private Vector2 oldOffset;
	private Vector2 oldSize;

	private BoxCollider2D collider;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		collider = null;

		//Get collider from wherever specified
		switch(getLevel)
		{
			case GetLevel.Self:
				collider = animator.GetComponent<BoxCollider2D>();
				break;
			case GetLevel.Parents:
				collider = animator.GetComponentInParent<BoxCollider2D>();
				break;
			case GetLevel.Children:
				collider = animator.GetComponentInChildren<BoxCollider2D>();
				break;
		}

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
