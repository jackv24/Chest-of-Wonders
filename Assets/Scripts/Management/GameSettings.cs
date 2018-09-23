using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class GameSettings : MonoBehaviour
{
	private static GameSettings instance;

	public enum VolumeTarget
	{
		Master,
		Music,
		Sound
	}

	private static readonly Dictionary<VolumeTarget, string> volumeKeys = new Dictionary<VolumeTarget, string>()
	{
		{ VolumeTarget.Master, "MasterVolume" },
		{ VolumeTarget.Music, "MusicVolume" },
		{ VolumeTarget.Sound, "SoundVolume" }
	};

	[SerializeField]
	private AudioMixer audioMixer;

	private void Awake()
	{
		if (instance != null)
			Destroy(this);
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		UpdateVolumeMixer();
	}

	public static float GetVolume(VolumeTarget volumeTarget)
	{
		return PlayerPrefs.GetFloat(volumeKeys[volumeTarget], 1.0f);
	}

	public static void SetVolume(VolumeTarget volumeTarget, float volume)
	{
		if (!instance)
			return;

		PlayerPrefs.SetFloat(volumeKeys[volumeTarget], volume);

		instance.UpdateVolumeMixer();
	}

	private void UpdateVolumeMixer()
	{
		if (!audioMixer)
			return;

		Array enumValues = Enum.GetValues(typeof(VolumeTarget));
		foreach(var value in enumValues)
		{
			var target = (VolumeTarget)value;
			audioMixer.SetFloat(volumeKeys[target], LinearToDecibel(GetVolume(target)));
		}
	}

	private static float LinearToDecibel(float linear)
	{
		float dB;

		if (linear != 0)
			dB = 20.0f * Mathf.Log10(linear);
		else
			dB = -144.0f;

		return dB;
	}
}
