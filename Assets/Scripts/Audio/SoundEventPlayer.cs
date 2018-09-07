using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventPlayer : MonoBehaviour
{
	[SerializeField]
	private SoundType type;

	[SerializeField]
	private SoundEventRandom sounds;

	[SerializeField]
	private bool playOnEnable;

	[SerializeField]
	private bool playOnDisable;

	private void OnEnable()
	{
		if (playOnEnable)
			Play();
	}

	private void OnDisable()
	{
		if (playOnDisable)
			Play();
	}

	public void Play()
	{
		sounds.Play(transform.position, type);
	}
}
