using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
	public float cooldownTime = 0.5f;
	private float nextDodgeTime;

	public float dodgeTime = 1.0f;

	private Coroutine dodgeRoutine = null;

	private CharacterAnimator characterAnimator;

	private void Awake()
	{
		characterAnimator = GetComponent<CharacterAnimator>();
	}

	public void Dodge(Vector2 direction)
	{
		if (dodgeRoutine == null && Time.time >= nextDodgeTime)
		{
			//Snap to * directions
			direction = Helper.SnapTo(direction, 45.0f).normalized;

			dodgeRoutine = StartCoroutine(DodgeRoutine(direction));
		}
	}

	IEnumerator DodgeRoutine(Vector2 direction)
	{
		//TODO: Flash white

		characterAnimator.Play("Dodge");

		//TODO: Cancel other inputs

		//TODO: Dodge in direction on ground

		//TODO: Dodge in direction in air (cancel gravity)

		yield return new WaitForSeconds(dodgeTime);

		characterAnimator.Play("Locomotion");

		nextDodgeTime = Time.time + cooldownTime;

		dodgeRoutine = null;
	}
}
