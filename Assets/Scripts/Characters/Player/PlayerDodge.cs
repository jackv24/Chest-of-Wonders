using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
	private enum DodgeType
	{
		Ground,
		Air
	}

	public float cooldownTime = 0.3f;
	private float nextDodgeTime;

	public float dodgeTime = 0.6f;

	public float dodgeSpeed = 10.0f;
	public AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

	private Coroutine dodgeRoutine = null;

	private CharacterAnimator characterAnimator;
	private CharacterStats characterStats;
	private CharacterMove characterMove;
	private PlayerInput playerInput;

	private void Awake()
	{
		characterAnimator = GetComponent<CharacterAnimator>();
		characterStats = GetComponent<CharacterStats>();
		characterMove = GetComponent<CharacterMove>();
		playerInput = GetComponent<PlayerInput>();
	}

	public void Dodge(Vector2 direction)
	{
		if (dodgeRoutine == null && Time.time >= nextDodgeTime)
		{
			//Snap to 8 directions
			direction = Helper.SnapTo(direction, 45.0f).normalized;

			dodgeRoutine = StartCoroutine(DodgeRoutine(direction));
		}
	}

	IEnumerator DodgeRoutine(Vector2 direction)
	{
		//TODO: Determine dodge type
		DodgeType type = DodgeType.Ground;

		//Start dodge
		characterStats.damageImmunity = true;
		playerInput.AcceptingInput = false;

		switch (type)
		{
			case DodgeType.Ground:
				characterAnimator.Play("Dodge");

				//Dodge in facing direction over time
				characterMove.Move(characterMove.FacingDirection);

				//Change speed over dodge time according to curve
				float initialSpeed = characterMove.moveSpeed;
				float elapsed = 0;
				while (elapsed <= dodgeTime)
				{
					characterMove.moveSpeed = dodgeSpeed * speedCurve.Evaluate(elapsed / dodgeTime);

					yield return null;
					elapsed += Time.deltaTime;
				}

				characterMove.moveSpeed = initialSpeed;
				break;

			case DodgeType.Air:
				//TODO: Dodge in direction in air (cancel gravity)
				yield return new WaitForSeconds(dodgeTime);
				break;
		}

		//Playing locomotion state will naturally transition out to other states
		characterAnimator.ReturnToLocomotion();

		//Return to previous state after dodge
		characterStats.damageImmunity = false;
		playerInput.AcceptingInput = true;

		nextDodgeTime = Time.time + cooldownTime;

		dodgeRoutine = null;
	}
}
