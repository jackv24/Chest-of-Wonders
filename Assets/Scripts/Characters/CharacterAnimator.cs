using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour
{
    [Tooltip("The Animator to use for animation (usually attached the a child graphic).")]
    public Animator animator;

    [Space()]
    [Tooltip("Does the character look towards the right by default? (used to flip the character to face the right move direction)")]
    public bool defaultRight = true;
    private float oldFaceDirection = 0;

	public bool handleFlip = true;

    [Space()]
    public AnimationClip deathAnimation;

    private CharacterMove characterMove;

    private void Awake()
    {
        //If there has been no animator assigned, log a warning
        if (!animator)
            Debug.LogWarning("No Animator assigned to CharacterAnimator on " + name);

        //Get references
        characterMove = GetComponent<CharacterMove>();
    }

    private void Start()
    {
        if (characterMove)
        {
			if(handleFlip)
				characterMove.OnChangedDirection += FlipOnDirectionChange;

            characterMove.OnJump += Jump;
        }
    }

    private void Update()
    {
        //If there has been an animator assigned
        if (animator)
        {
            if (characterMove)
            {
                //Get horizontal move speed
                float horizontal = characterMove.InputDirection;

                //Set property on animator
                animator.SetFloat("horizontal", horizontal);

                animator.SetBool("isGrounded", characterMove.IsGrounded);
            }
        }
    }

    void FlipOnDirectionChange(float direction)
    {
		if (direction != oldFaceDirection && direction != 0)
		{
			oldFaceDirection = direction;

			//Flip character to face the right direction
			//Get current scale
			Vector3 scale = animator.transform.localScale;

			//Edit x scale to flip character
			if (direction >= 0)
				scale.x = defaultRight ? 1 : -1;
			else
				scale.x = defaultRight ? -1 : 1;

			//Set new scale as current scale
			animator.transform.localScale = scale;
		}
	}

    public void SetStunned(bool value)
    {
        if(value == true)
            animator.SetTrigger("stun");

        animator.SetBool("stunned", value);
    }

    void Jump()
    {
        animator.SetTrigger("jump");
    }

    public bool Death()
    {
        if (ContainsParameter("isAlive"))
        {
            animator.SetBool("stunned", false);
            animator.SetBool("isAlive", false);

            StartCoroutine("DisableAfterDeathAnim");

            return true;
        }
        else
            return false;
    }

    IEnumerator DisableAfterDeathAnim()
    {
        if(deathAnimation)
        {
            yield return new WaitForSeconds(deathAnimation.length);

            gameObject.SetActive(false);
        }
    }

    bool ContainsParameter(string name)
    {
        foreach(AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == name)
                return true;
        }

        return false;
    }

	/// <summary>
	/// Wrapper function for easily playing animations on the assigned animator
	/// </summary>
	/// <param name="stateName">The name of the state to play.</param>
	public void Play(string stateName)
	{
		animator?.Play(stateName);
	}

	/// <summary>
	/// Plays an animation state and invokes an action upon completion.
	/// </summary>
	/// <param name="stateName">The name of the state to play.</param>
	/// <param name="completionAction">The action to invoke upon completion of the animation.</param>
	public void Play(string stateName, System.Action completionAction)
	{
		if(animator)
			StartCoroutine(PlayAnimWait(stateName, completionAction));
	}

	private IEnumerator PlayAnimWait(string stateName, System.Action completionAction)
	{
		AnimatorStateInfo previousState = animator.GetCurrentAnimatorStateInfo(0);

		animator.Play(stateName);
		yield return null;

		AnimatorStateInfo newState = animator.GetCurrentAnimatorStateInfo(0);

		//If the current state has not changed then the animation must have failed to play
		if (newState.shortNameHash != previousState.shortNameHash)
			yield return new WaitForSeconds(newState.length);
		else
			Debug.LogWarning($"Character Animation State {stateName} could not be played!");

		completionAction?.Invoke();
	}

	/// <summary>
	/// Plays the locomation animator state. Useful when returning from a custom animation, as the locomotion state will naturally transition out when needed.
	/// </summary>
	public void ReturnToLocomotion()
	{
		Play("Locomotion");
	}

	/// <summary>
	/// Checks whether the character's animator is in any of the specified states
	/// </summary>
	public bool IsInState(params string[] stateNames)
	{
		if(animator)
		{
			foreach(string stateName in stateNames)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
					return true;
			}
		}

		return false;
	}
}
