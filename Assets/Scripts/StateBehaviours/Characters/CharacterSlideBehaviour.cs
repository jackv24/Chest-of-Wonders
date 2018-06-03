using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSlideBehaviour : CharacterStateBehaviour
{
	public float moveSpeed = 10.0f;
	public AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

	private float direction;
	private float initialMoveSpeed;

	[Space()]
	public GameObject particleEffectPrefab;
	private GameObject particleEffect;

	public Vector2 particleOffset = new Vector2(0, 0.25f);

	public float particleLeftRotation = 150;
	public float particleRightRotation = 0;

	private CharacterMove characterMove;
	private CharacterStats characterStats;
	private PlayerInput playerInput;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);

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

		if(particleEffectPrefab)
		{
			particleEffect = ObjectPooler.GetPooledObject(particleEffectPrefab);
			particleEffect.transform.position = animator.transform.position + new Vector3(particleOffset.x * direction, particleOffset.y);

			Vector3 eulerAngles = particleEffect.transform.eulerAngles;
			eulerAngles.z = direction < 0 ? particleLeftRotation : particleRightRotation;
			particleEffect.transform.eulerAngles = eulerAngles;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		if (!hasEnded)
		{
			if (characterMove)
			{
				characterMove.moveSpeed = moveSpeed * speedCurve.Evaluate(normalizedTime);

				characterMove.Move(direction);
			}

			if(particleEffect)
				particleEffect.transform.position = animator.transform.position + new Vector3(particleOffset.x * direction, particleOffset.y);
		}
	}

	protected override void EndBehaviour()
	{
		base.EndBehaviour();

		if (characterMove)
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
