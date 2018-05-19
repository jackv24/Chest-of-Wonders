﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
	[Serializable]
	public struct DodgeDetails
	{
		public float duration;
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

	public SoundEvent dodgeSound;

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

	public void Dodge(Vector2 direction)
	{
		if (dodgeRoutine == null && Time.time >= nextDodgeTime)
		{
			//Determine dodge type
			DodgeType type = characterMove.isGrounded ? DodgeType.Ground : DodgeType.Air;

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

	IEnumerator DodgeRoutine(DodgeType type, Vector2 direction)
	{
		//Cannot dodge without a direction
		if (direction.magnitude < 0.01f)
			direction = new Vector2(characterMove.FacingDirection, 0);

		//Start dodge
		characterStats.damageImmunity = true;
		playerInput.AcceptingInput = false;

		dodgeSound.Play(transform.position);

		switch (type)
		{
			case DodgeType.Ground:
				{
					characterAnimator.Play("Dodge");

					//Dodge in facing direction over time
					characterMove.Move(Mathf.Sign(direction.x));

					//Change speed over dodge time according to curve
					float initialSpeed = characterMove.moveSpeed;
					float elapsed = 0;
					while (elapsed <= groundDodge.duration)
					{
						characterMove.moveSpeed = groundDodge.speed * groundDodge.speedCurve.Evaluate(elapsed / groundDodge.duration);

						yield return null;
						elapsed += Time.deltaTime;
					}

					characterMove.moveSpeed = initialSpeed;
				}
				break;

			case DodgeType.Air:
				{
					characterAnimator.Play("Dodge Air");

					//Switch character control from script to rigidbody (script is made for ground movement)
					characterMove.SwitchToRigidbody();
					float gravity = characterMove.body.gravityScale;
					characterMove.body.gravityScale = 0;

					//Snap to 8 directions
					direction = Helper.SnapTo(direction, 45.0f).normalized;

					float elapsed = 0;
					while (elapsed <= airDodge.duration)
					{
						//Reset velocity every fixed update in case we hit something (so we're not knocked around, just merely stopped)
						characterMove.body.velocity = direction * airDodge.speed * airDodge.speedCurve.Evaluate(elapsed / airDodge.duration);

						//TODO: Break when hit ground

						//Use fixed delta time since we're using a rigidbody
						yield return new WaitForFixedUpdate();
						elapsed += Time.fixedDeltaTime;
					}

					//Return values and script control back to normal
					characterMove.body.gravityScale = gravity;
					characterMove.SwitchBackFromRigidbody();
				}
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
