using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSlideBehaviour : CharacterStateBehaviour
{
	public float moveSpeed = 10.0f;
	public AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

	private float direction;
	private float initialMoveSpeed;

	private CharacterMove characterMove;
	private CharacterStats characterStats;
	private PlayerInput playerInput;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//GetComponents on state enter (will be less expensive on subsequent entries due to caching)
		characterMove = GetComponentAtLevel<CharacterMove>(animator.gameObject);
		characterStats = GetComponentAtLevel<CharacterStats>(animator.gameObject);
		playerInput = GetComponentAtLevel<PlayerInput>(animator.gameObject);

		if (characterMove)
		{
			direction = characterMove.FacingDirection;
			initialMoveSpeed = characterMove.moveSpeed;
		}

		if(characterStats)
			characterStats.damageImmunity = true;

		//The character is not necessarily a player
		if (playerInput)
			playerInput.AcceptingInput = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(characterMove)
		{
			characterMove.moveSpeed = moveSpeed * speedCurve.Evaluate(stateInfo.normalizedTime);

			characterMove.Move(direction);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(characterMove)
		{
			characterMove.moveSpeed = initialMoveSpeed;
			characterMove.Move(0);
		}

		if (characterStats)
			characterStats.damageImmunity = false;

		if (playerInput)
			playerInput.AcceptingInput = true;
	}
}
