using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour
{
    [Tooltip("The Animator to use for animation (usually attached the a child graphic).")]
    public Animator animator;

    [Space()]
    [Tooltip("Does the character look towards the right by default? (used to flip the character to face the right move direction)")]
    public bool defaultRight = true;

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
                float horizontal = characterMove.inputDirection;

                //Set property on animator
                animator.SetFloat("horizontal", horizontal);

                animator.SetBool("isGrounded", characterMove.isGrounded);
            }
        }
    }

    void FlipOnDirectionChange(float direction)
    {
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

    public void SetStunned(bool value)
    {
        if(value == true)
            animator.SetTrigger("stun");

        animator.SetBool("stunned", value);
    }

    public void MeleeAttack(bool holding, float verticalDirection)
    {
        if(!holding)
            animator.SetFloat("vertical", verticalDirection);

        animator.SetBool("holdingBat", holding);
    }

    public void SetCharged(bool value)
    {
        animator.SetBool("charged", value);
    }

    public void SetAbsorbing(bool value)
    {
        animator.SetBool("absorbing", value);
    }

    public void MagicAttack()
    {
        animator.SetTrigger("magic");
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
}
