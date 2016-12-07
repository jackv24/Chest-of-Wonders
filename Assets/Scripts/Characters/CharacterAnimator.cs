using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour
{
    [Tooltip("The Animator to use for animation (usually attached the a child graphic).")]
    public Animator animator;

    [Space()]
    [Tooltip("Does the character look towards the right by default? (used to flip the character to face the right move direction)")]
    public bool defaultRight = true;

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
        if(characterMove)
            characterMove.OnChangedDirection += FlipOnDirectionChange;
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

    public void PrimaryAttack()
    {
        animator.SetTrigger("attack1");
    }

    public void SecondaryAttack()
    {
        animator.SetTrigger("attack2");
    }
}
