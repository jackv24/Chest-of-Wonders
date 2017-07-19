using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectBase : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public AudioClip clip;
        public float volume;

        public SoundEffect()
        {
            volume = 1.0f;
        }
    }

	private float initialVolume;

    private AudioSource source;

    void Awake()
    {
        if(!source)
            source = GetComponent<AudioSource>();

        if(!source)
            Debug.LogWarning("No AudioSource attached to " + gameObject.name);
    }

	private void Start()
	{
		if(source)
			initialVolume = source.volume;
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

	public void PlaySound(SoundEffect soundEffect, bool setLoop)
	{
		if (source)
		{
			if (soundEffect.clip)
			{
				if(!setLoop)
					source.PlayOneShot(soundEffect.clip, soundEffect.volume);
				else
				{
					source.clip = soundEffect.clip;
					source.loop = true;
					source.volume = soundEffect.volume;
					source.Play();
				}
			}
		}
	}

	public void ClearLoop()
	{
		if(source)
		{
			source.clip = null;
			source.volume = initialVolume;
		}
	}
}
