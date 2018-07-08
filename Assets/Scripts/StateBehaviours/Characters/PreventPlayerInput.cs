using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventPlayerInput : CharacterStateBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GetComponentAtLevel<PlayerInput>(animator.gameObject).AcceptingInput = false;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GetComponentAtLevel<PlayerInput>(animator.gameObject).AcceptingInput = true;
	}
}
