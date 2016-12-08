using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private CharacterAnimator characterAnimator;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    //Easy use function to use the current primary and secondary attacks (called from input class)
    public void UsePrimary()
    {
        if (characterAnimator)
            characterAnimator.PrimaryAttack();
    }
    public void UseSecondary()
    {
        if (characterAnimator)
            characterAnimator.SecondaryAttack();
    }
}
