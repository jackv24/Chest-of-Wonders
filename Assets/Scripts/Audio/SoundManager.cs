using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
	Misc,
	UI,
	Player,
	Enemy
}

[System.Serializable]
public class SoundEventBasic
{
	public AudioClip clip;
	public float volume = 1.0f;
	public MinMaxFloat pitchRange = new MinMaxFloat(1.0f, 1.0f);

	/// <summary>
	/// Will play this sound using the SoundManager (which should always be present), and using a specified type. NOTE: Use a regular SoundEvent if you need to assign the type in the inspector.
	/// </summary>
	/// <param name="position">The position at which to spawn the AudioSource.</param>
	/// <param name="type">The types this sound is, controls which audio mixer channel it is played through.</param>
	public void Play(Vector2 position, SoundType type)
	{
		SoundEvent soundEvent = new SoundEvent
		{
			clip = clip,
			volume = volume,
			pitchRange = pitchRange,
			type = type
		};

		SoundManager.Instance?.PlaySound(soundEvent, position);
	}
}

[System.Serializable]
public class SoundEvent : SoundEventBasic
{
	public SoundType type;

	/// <summary>
	/// Will play this sound using the SoundManager (which should always be present).
	/// </summary>
	/// <param name="position">The position at which to spawn the AudioSource.</param>
	public void Play(Vector2 position)
	{
		SoundManager.Instance?.PlaySound(this, position);
	}
}

/// <summary>
/// A super easy and robust way to play one-off sounds without needing to attach an AudioSource to everything.
/// Should be attached to the GameManager so it is always present.
/// </summary>
public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;

	public AudioSource miscAudioSourcePrefab;
	public AudioSource uiAudioSourcePrefab;
	public AudioSource playerAudioSourcePrefab;
	public AudioSource enemyAudioSourcePrefab;

	private void Awake()
	{
		Instance = this;
	}

	public void PlaySound(SoundEvent sound, Vector2 position)
	{
		//Can't play a sound if none exists, and no point if you can't hear it
		if (sound.clip == null || sound.volume <= 0)
			return;

		AudioSource audioSourcePrefab = null;

		//Get the correct audio source prefab for this type (they have different mixer groups, etc)
		switch(sound.type)
		{
			case SoundType.Misc:
				audioSourcePrefab = miscAudioSourcePrefab;
				break;
			case SoundType.UI:
				audioSourcePrefab = uiAudioSourcePrefab;
				break;
			case SoundType.Player:
				audioSourcePrefab = playerAudioSourcePrefab;
				break;
			case SoundType.Enemy:
				audioSourcePrefab = enemyAudioSourcePrefab;
				break;
		}

		if (audioSourcePrefab)
		{
			GameObject obj = ObjectPooler.GetPooledObject(audioSourcePrefab.gameObject);
			obj.transform.position = position;

			AudioSource source = obj.GetComponent<AudioSource>(); //Don't bother null-checking here since we're guaranteed there will be an AudioSource attached

			//Set AudioSource parameters from SoundEvent
			source.clip = sound.clip;
			source.volume = sound.volume;
			source.pitch = sound.pitchRange.RandomValue;

			source.Play();

			//Recycle the spawned AudioSource after its clip has played
			StartCoroutine(RecycleAudioSource(sound.clip.length, obj));
		}
		else
			Debug.LogError("No audio source prefab was found for " + sound.type, this);
	}

	IEnumerator RecycleAudioSource(float delay, GameObject obj)
	{
		yield return new WaitForSeconds(delay);

		obj.SetActive(false);
	}
}
