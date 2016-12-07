using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public CharacterMove characterMove;

    public void AllowMovement()
    {
        if (characterMove)
            characterMove.canMove = true;
    }

    public void DisallowMovement()
    {
        if (characterMove)
            characterMove.canMove = false;
    }
}
