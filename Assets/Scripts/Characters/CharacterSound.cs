using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : SoundEffectBase
{
    public SoundEffect jumpSound;
    public SoundEffect landSound;
    public SoundEffect hurtSound;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    private bool playingFootsteps;
    public SoundEffect[] footsteps;

    [Space()]
    public float footstepInterval = 0.1f;

    [Space()]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minVolume = 0.7f;
    public float maxVolume = 1.1f;

    public void PlayFootstep()
    {
        if (footstepSource)
        {
            int index = 0;

            //TODO: Get tile and sound to play

            if (index >= 0 && index < footsteps.Length)
            {
                footstepSource.clip = footsteps[index].clip;
                footstepSource.volume = footsteps[index].volume;

                footstepSource.pitch = Random.Range(minPitch, maxPitch);

                float volume = Random.Range(minVolume, maxVolume);

                footstepSource.PlayOneShot(footsteps[index].clip, volume);
            }
        }
    }
}
