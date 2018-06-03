using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSetVelocity : CharacterStateBehaviour
{
	public Vector2 velocity;

	public float inheritVelocityX = 0;
	private float inheritedVelocityX;

	public float inheritVelocityY = 0;

	private float inheritedVelocityY;
	public AnimationCurve multiplierX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public AnimationCurve multiplierY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

	private float directionX;

	private CharacterMove characterMove;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);

		characterMove = GetComponentAtLevel<CharacterMove>(animator.gameObject);

		if(characterMove)
		{
			directionX = characterMove.FacingDirection;

			//Use absolute X velocity since we're multiplying in facing direction later
			inheritedVelocityX = Mathf.Abs(characterMove.Velocity.x) * inheritVelocityX;
			inheritedVelocityY = characterMove.Velocity.y * inheritVelocityY;

			characterMove.MovementState = CharacterMovementStates.SetVelocity;
			characterMove.ResetJump();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		if (!hasEnded)
		{
			if (characterMove)
			{
				Vector2 newVelocity = velocity;

				newVelocity.x += inheritedVelocityX;
				newVelocity.y += inheritedVelocityY;

				newVelocity.x *= multiplierX.Evaluate(normalizedTime) * directionX;
				newVelocity.y *= multiplierY.Evaluate(normalizedTime);

				characterMove.Velocity = newVelocity;
			}
		}
	}

	protected override void EndBehaviour()
	{
		base.EndBehaviour();

		if(characterMove)
		{
			characterMove.Velocity = Vector2.zero;
			characterMove.MovementState = CharacterMovementStates.Normal;
		}
	}
}
