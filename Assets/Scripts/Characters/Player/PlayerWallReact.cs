using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallReact : MonoBehaviour
{
    private int wallBonkHash;

    private bool alreadyBonked;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;
    private Animator animator;

    private void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Start()
    {
        animator = characterAnimator.Animator;

        wallBonkHash = Animator.StringToHash("wallBonk");

        characterMove.OnChangedDirection += (dir) => alreadyBonked = false;
    }

    private void LateUpdate()
    {
        if (alreadyBonked)
            return;

        // Get in LateUpdate after CharacterMove has populated arrays, so we can react in the same frame
        bool didHit = false;
        if (characterMove.InputDirection != 0)
        {
            RaycastHit2D[] horizontalRayHits = characterMove.HorizontalRaycastHits;
            foreach (var rayHit in horizontalRayHits)
            {
                if (rayHit.collider != null)
                {
                    didHit = true;
                    break;
                }
            }
        }

        if (didHit)
        {
            animator.SetTrigger(wallBonkHash);
            alreadyBonked = true;
        }
        else
            animator.ResetTrigger(wallBonkHash);
    }
}
