using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectBase : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public AudioClip clip;
        public float volume = 1.0f;
    }

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if(!source)
            Debug.LogWarning("No AudioSource attached to " + gameObject.name);
    }

    public void PlaySound(SoundEffect soundEffect)
    {
        if (source)
        {
            if (soundEffect.clip)
            {
                source.PlayOneShot(soundEffect.clip, soundEffect.volume);
            }
        }
    }
}
