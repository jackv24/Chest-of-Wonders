using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : SoundEffectBase
{
    public SoundEffect jumpSound;
    public SoundEffect landSound;
    public SoundEffect hurtSound;
    public SoundEffect deathSound;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    private bool playingFootsteps;
    public SoundEffect footsteps;

    [Space()]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minVolume = 0.7f;
    public float maxVolume = 1.1f;

    public void PlayFootstep()
    {
        if (footstepSource)
        {
            footstepSource.clip = footsteps.clip;
            footstepSource.volume = footsteps.volume;

            footstepSource.pitch = Random.Range(minPitch, maxPitch);

            float volume = Random.Range(minVolume, maxVolume);

            footstepSource.PlayOneShot(footsteps.clip, volume);
        }
    }
}
