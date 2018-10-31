using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventCharacterTurn : CachedStateBehaviour
{
    [SerializeField]
    private bool enableOnExit = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetComponentAtLevel<CharacterMove>(animator.gameObject).CanChangeDirection = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enableOnExit)
            GetComponentAtLevel<CharacterMove>(animator.gameObject).CanChangeDirection = true;
    }
}
