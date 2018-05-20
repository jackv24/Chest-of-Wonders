using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationZ : StateMachineBehaviour
{
	public float rotation;

	private float previousRotation;
	private Vector2 previousPosition;

	public bool resetOnExit = true;
	public bool flipSignWithXScale = true;
	public bool rotateAroundCentre = true;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		previousRotation = animator.transform.localEulerAngles.z;
		previousPosition = animator.transform.localPosition;

		float newAngle = rotation * (flipSignWithXScale ? animator.transform.localScale.x : 1);

		if (rotateAroundCentre)
		{
			//Get centre of sprite, else just use position
			Vector3 centre = animator.transform.GetComponent<Renderer>()?.bounds.center ?? animator.transform.position;

			animator.transform.RotateAround(centre, -Vector3.forward, newAngle);
		}
		else
		{
			animator.transform.SetRotationZ(newAngle);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(resetOnExit)
		{
			animator.transform.SetRotationZ(previousRotation);
			animator.transform.localPosition = previousPosition;
		}
	}
}
