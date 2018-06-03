using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
	[Serializable]
	public struct DodgeDetails
	{
		public float speed;
		public AnimationCurve speedCurve;
	}

	public enum DodgeType
	{
		Ground,
		Air
	}

	public DodgeDetails groundDodge;
	public DodgeDetails airDodge;

	public float cooldownTime = 0.3f;
	private float nextDodgeTime;

	public int maxAirDodges = 1;
	private int airDodgesLeft;

	private Coroutine dodgeRoutine = null;

	public SoundEventType dodgeSound;

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

	private void Start()
	{
		if (characterMove)
			characterMove.OnGrounded += () => { airDodgesLeft = maxAirDodges; };

		airDodgesLeft = maxAirDodges;
	}

	private void OnDisable()
	{
		//If still in dodge routine when disabling, return to regular state first
		if(dodgeRoutine != null)
		{
			EndDodge(false);

			dodgeRoutine = null;
		}
	}

	public void Dodge(Vector2 direction)
	{
		//Can only dodge if in a regular state (can't dodge in the middle of an attack, etc)
		if (!characterAnimator || characterAnimator.IsInState("Locomotion", "Jump", "Fall", "Land"))
		{
			if (dodgeRoutine == null && Time.time >= nextDodgeTime)
			{
				//Determine dodge type
				DodgeType type = characterMove.IsGrounded ? DodgeType.Ground : DodgeType.Air;

				//Limit air dodges
				if (type == DodgeType.Air)
				{
					if (airDodgesLeft <= 0)
						return;

					airDodgesLeft--;
				}

				dodgeRoutine = StartCoroutine(DodgeRoutine(type, direction));
			}
		}
	}

	IEnumerator DodgeRoutine(DodgeType type, Vector2 direction)
	{
		//Can only dodge horizontally on ground
		if(type == DodgeType.Ground)
		{
			direction.y = 0;
			direction.Normalize();
		}

		//Cannot dodge without a direction
		if (direction.magnitude < 0.01f)
			direction = new Vector2(characterMove.FacingDirection, 0);

		//Snap to 8 directions
		direction = Helper.SnapTo(direction, 45.0f).normalized;

		//Determine animation to play (ground is side animation, air is directional)
		string dodgeAnim = "Dodge Side";
		if(type == DodgeType.Air)
		{
			if (direction == Vector2.up)
				dodgeAnim = "Dodge Up";
			else if (direction == Vector2.down)
				dodgeAnim = "Dodge Down";
			else if (direction.x != 0)
			{
				if (direction.y > 0)
					dodgeAnim = "Dodge Diagonal Up";
				else if (direction.y < 0)
					dodgeAnim = "Dodge Diagonal Down";
			}
			//Horizontal directions are already default, so no need to add a case for them
		}

		//Play animation and set length to wait
		characterAnimator?.Play(dodgeAnim);
		yield return null;
		float duration = characterAnimator.animator.GetCurrentAnimatorStateInfo(0).length;

		//Start dodge
		characterStats.damageImmunity = true;
		playerInput.AcceptingInput = false;

		characterMove.MovementState = CharacterMovementStates.SetVelocity;
		characterMove.Velocity = Vector2.zero;
		characterMove.ResetJump();

		dodgeSound.Play(transform.position);

		bool returnToLocomotion = true;

		switch (type)
		{
			case DodgeType.Ground:
				{
					//Change speed over dodge time according to curve
					float elapsed = 0;
					while (elapsed <= duration)
					{
						characterMove.Velocity = direction * groundDodge.speed * groundDodge.speedCurve.Evaluate(elapsed / duration);

						yield return null;
						elapsed += Time.deltaTime;
					}
				}
				break;

			case DodgeType.Air:
				{
					float elapsed = 0;
					while (elapsed <= duration)
					{
						//Set velocity every frame according to curve
						characterMove.Velocity = direction * airDodge.speed * airDodge.speedCurve.Evaluate(elapsed / duration);

						//Land immediately and cancel when dodging into ground
						if (characterMove.IsGrounded && characterMove.Velocity.y < 0)
						{
							characterAnimator?.Play("Land");
							returnToLocomotion = false;
							break;
						}

						yield return null;
						elapsed += Time.deltaTime;
					}
				}
				break;
		}

		EndDodge(returnToLocomotion);

		dodgeRoutine = null;
	}

	private void EndDodge(bool changeAnim)
	{
		characterMove.MovementState = CharacterMovementStates.Normal;

		//Playing locomotion state will naturally transition out to other states
		if (changeAnim)
			characterAnimator.ReturnToLocomotion();

		//Return to previous state after dodge
		characterStats.damageImmunity = false;
		playerInput.AcceptingInput = true;

		nextDodgeTime = Time.time + cooldownTime;
	}
}
