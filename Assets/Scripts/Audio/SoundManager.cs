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

public abstract class SoundEventBase
{
	//Ther inheriting class can implements this however they like (single clip, randomly selected, etc)
	public abstract AudioClip Clip { get; }

	public float volume = 1.0f;
	public MinMaxFloat pitchRange = new MinMaxFloat(1.0f, 1.0f);

	/// <summary>
	/// Will play this sound using the SoundManager (which should always be present), and using a specified type. NOTE: Use a regular SoundEvent if you need to assign the type in the inspector.
	/// </summary>
	/// <param name="position">The position at which to spawn the AudioSource.</param>
	/// <param name="type">The types this sound is, controls which audio mixer channel it is played through.</param>
	public void Play(Vector2 position, SoundType type)
	{
		SoundManager.Instance?.PlaySound(this, position, type);
	}
}

/// <summary>
/// A SoundEvent which will play a single audio clip (type must be passed in to the Play method)
/// </summary>
[System.Serializable]
public class SoundEventSingle : SoundEventBase
{
	[SerializeField] private AudioClip clip;

	public override AudioClip Clip { get { return clip; } }
}

/// <summary>
/// A SoundEvent which will play a random clip from an array (type must be passed in to the Play method)
/// </summary>
[System.Serializable]
public class SoundEventRandom : SoundEventBase
{
	[SerializeField] private AudioClip[] clips;

	public override AudioClip Clip { get { return clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null; } }
}

/// <summary>
/// A SoundEvent which will play a single audio clip of a specified type
/// </summary>
[System.Serializable]
public class SoundEventType : SoundEventSingle
{
	public SoundType type;

	/// <summary>
	/// Will play this sound using the SoundManager (which should always be present).
	/// </summary>
	/// <param name="position">The position at which to spawn the AudioSource.</param>
	public void Play(Vector2 position)
	{
		SoundManager.Instance?.PlaySound(this, position, type);
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

	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Spawns a temporary (pooled) audio source to play a SoundEvent.
	/// </summary>
	/// <param name="sound">The SoundEvent to play.</param>
	/// <param name="position">The position at whioch to spawn the AudioSource.</param>
	/// <param name="type">The type that the sound is (controls which mixer channel to play through).</param>
	public void PlaySound(SoundEventBase sound, Vector2 position, SoundType type)
	{
		PlaySound(sound.Clip, sound.volume, sound.pitchRange.RandomValue, position, type);
	}

	/// <summary>
	/// Spawns a temporary audio source to play a sound (use other overloaded method to play a SoundEvent).
	/// </summary>
	public void PlaySound(AudioClip clip, float volume, float pitch, Vector2 position, SoundType type)
	{
		//Can't play a sound if none exists, and no point if you can't hear it
		if (clip == null || volume <= 0)
			return;

		AudioSource audioSourcePrefab = null;

		//Get the correct audio source prefab for this type (they have different mixer groups, etc)
		switch(type)
		{
			case SoundType.Enemy:
			case SoundType.Misc:
				audioSourcePrefab = miscAudioSourcePrefab;
				break;
			case SoundType.UI:
				audioSourcePrefab = uiAudioSourcePrefab;
				break;
			case SoundType.Player:
				audioSourcePrefab = playerAudioSourcePrefab;
				break;
		}

		if (audioSourcePrefab)
		{
			GameObject obj = ObjectPooler.GetPooledObject(audioSourcePrefab.gameObject);
			obj.transform.position = position;

			AudioSource source = obj.GetComponent<AudioSource>(); //Don't bother null-checking here since we're guaranteed there will be an AudioSource attached

			//Set AudioSource parameters from SoundEvent
			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;

			source.Play();

			//Recycle the spawned AudioSource after its clip has played
			StartCoroutine(RecycleAudioSource(clip.length, obj));
		}
		else
			Debug.LogError("No audio source prefab was found for " + type, this);
	}

	IEnumerator RecycleAudioSource(float delay, GameObject obj)
	{
		yield return new WaitForSeconds(delay);

		obj.SetActive(false);
	}
}
