using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private AnimationCurve speedCurve;

    [SerializeField]
    private float cooldownTime = 0.3f;
	private float nextDodgeTime;

    [SerializeField]
    private int maxAirDodges = 1;
	private int airDodgesLeft;

    public bool IsDodging { get { return dodgeRoutine != null; } }

    [SerializeField]
    private SpriteAfterImageEffect trailEffect;

	private Coroutine dodgeRoutine = null;

    [SerializeField]
    private SoundEventType dodgeSound;

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
        // Cannot dodge on ground
        if (characterMove.IsGrounded)
            return;

		//Can only dodge if in a regular state (can't dodge in the middle of an attack, etc)
		if (!characterAnimator || characterAnimator.IsInState("Locomotion", "Jump", "Fall", "Land", "Turn"))
		{
			if (dodgeRoutine == null && Time.time >= nextDodgeTime)
			{
				if (airDodgesLeft <= 0)
					return;

				airDodgesLeft--;

				dodgeRoutine = StartCoroutine(DodgeRoutine(direction));
			}
		}
	}

	IEnumerator DodgeRoutine(Vector2 direction)
	{
		//Cannot dodge without a direction
		if (direction.magnitude < 0.01f)
			direction = new Vector2(characterMove.FacingDirection, 0);

		//Snap to 4 directions
		direction = Helper.SnapTo(direction, 90.0f).normalized;

		//Determine animation to play
		string dodgeAnim = "Dodge Side";
		if (direction == Vector2.up)
			dodgeAnim = "Dodge Up";
		else if (direction == Vector2.down)
			dodgeAnim = "Dodge Down";

		//Play animation and set length to wait
		characterAnimator?.Play(dodgeAnim);
		yield return null;
		float duration = characterAnimator.Animator.GetCurrentAnimatorStateInfo(0).length;

		//Start dodge
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.None;

		characterMove.MovementState = CharacterMovementStates.SetVelocity;
		characterMove.Velocity = Vector2.zero;
		characterMove.ResetJump();

		dodgeSound.Play(transform.position);

		bool returnToLocomotion = true;

        float elapsed = 0;
        while (elapsed <= duration)
        {
            //Set velocity every frame according to curve
            characterMove.Velocity = direction * speed * speedCurve.Evaluate(elapsed / duration);

            //Land immediately and cancel when dodging into ground
            if (characterMove.IsGrounded && characterMove.Velocity.y < 0)
            {
                if (characterAnimator)
                    characterAnimator.Play("Land");

                returnToLocomotion = false;
                break;
            }

            yield return null;
            elapsed += Time.deltaTime;
        }

        EndDodge(returnToLocomotion);
	}

	public void EndDodge(bool changeAnim)
	{
        if (dodgeRoutine == null)
            return;

        StopCoroutine(dodgeRoutine);
        dodgeRoutine = null;

		characterMove.MovementState = CharacterMovementStates.Normal;

		//Playing locomotion state will naturally transition out to other states
		if (changeAnim)
			characterAnimator.ReturnToLocomotion();

		//Return to previous state after dodge
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;

		nextDodgeTime = Time.time + cooldownTime;

        if (trailEffect)
            trailEffect.EndAfterImageEffect();
	}
}
