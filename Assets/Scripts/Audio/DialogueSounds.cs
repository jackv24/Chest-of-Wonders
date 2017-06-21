using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSounds : SoundEffectBase
{
    [Header("Sounds")]
    public SoundEffect blip;
    [Space()]
    public SoundEffect shock;
    public SoundEffect nothing;

    public void PlaySound(string name)
    {
        SoundEffect sound = null;

        switch(name)
        {
            case "blip":
                sound = blip;
                break;
            case "shock":
                sound = shock;
                break;
            case "nothing":
                sound = nothing;
                break;
        }

        PlaySound(sound);
    }
}
