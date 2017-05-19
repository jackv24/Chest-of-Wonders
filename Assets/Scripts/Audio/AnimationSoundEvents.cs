using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvents : MonoBehaviour
{
    public CharacterSound characterSound;

    public void PlayFootstep()
    {
        if (characterSound)
        {
            characterSound.PlayFootstep();
        }
    }
}
