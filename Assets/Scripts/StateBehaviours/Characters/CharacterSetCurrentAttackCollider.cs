using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSetCurrentAttackCollider : StateMachineBehaviour
{
	public string colliderID;

	public bool setActive;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CharacterAnimationEvents events = animator.GetComponent<CharacterAnimationEvents>();

		if(events)
		{
			events.SetCurrentAttackCollider(colliderID);

			if (setActive)
				events.EnableCurrentAttackCollider();
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CharacterAnimationEvents events = animator.GetComponent<CharacterAnimationEvents>();

		if (events)
		{
			events.DisableCurrentAttackCollider();
			events.SetCurrentAttackCollider(null);
		}
	}
}
