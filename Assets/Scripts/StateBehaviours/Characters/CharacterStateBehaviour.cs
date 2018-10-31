using System.Collections;
using UnityEngine;

/// <summary>
/// A base class for StateMachineBehaviours that are intended to be used with character animations
/// </summary>
public abstract class CharacterStateBehaviour : CachedStateBehaviour
{
	//Behaviour can end before end of state
	[Range(0, 1.0f)]
	public float endTime = 1.0f;
	protected bool hasEnded;

	protected float normalizedTime;

	protected CharacterStats characterStats;
	protected PlayerInput playerInput; //A character can be controlled by the player, but doesn't need to be

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		characterStats = GetComponentAtLevel<CharacterStats>(animator.gameObject);
		playerInput = GetComponentAtLevel<PlayerInput>(animator.gameObject);

		if (characterStats)
			characterStats.damageImmunity = true;

		if (playerInput)
			playerInput.AcceptingInput = PlayerInput.InputAcceptance.None;

		hasEnded = false;

		normalizedTime = 0;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= endTime)
			EndBehaviour();

		//Remap state normalised time to accoutn for behaviour ending early
		normalizedTime = Mathf.Clamp(stateInfo.normalizedTime / endTime, 0, 1.0f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EndBehaviour();
	}

	protected virtual void EndBehaviour()
	{
		if (hasEnded)
			return;
		hasEnded = true;

		if (characterStats)
			characterStats.damageImmunity = false;

		if (playerInput)
			playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
	}
}
