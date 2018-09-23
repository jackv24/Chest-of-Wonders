using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class OptionsUI : MonoBehaviour
{
	[SerializeField]
	private Selectable firstSelected;

	[Space()]
	[SerializeField]
	private Dropdown resolutionDropdown;

	[SerializeField]
	private Toggle fullscreenToggle;

	private List<Resolution> resolutions;

	[SerializeField, ArrayForEnum(typeof(GameSettings.VolumeTarget))]
	private Slider[] volumeSliders;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref volumeSliders, typeof(GameSettings.VolumeTarget));
	}

	private void Start()
	{
		if(resolutionDropdown)
		{
			//Get list of available resolutions
			resolutions = new List<Resolution>(Screen.resolutions);

			//Create array of strings to match resolution array
			string[] resolutionStrings = new string[resolutions.Count];

			for (int i = 0; i < resolutions.Count; i++)
				resolutionStrings[i] = resolutions[i].ToString();

			//Clear current dropdown options before adding new ones
			resolutionDropdown.ClearOptions();

			resolutionDropdown.AddOptions(new List<string>(resolutionStrings));
			resolutionDropdown.value = resolutions.IndexOf(Screen.currentResolution);

			resolutionDropdown.onValueChanged.AddListener(delegate { UpdateResolution(); });

			if(fullscreenToggle)
			{
				fullscreenToggle.isOn = Screen.fullScreen;

				fullscreenToggle.onValueChanged.AddListener(delegate { UpdateResolution(); });
			}
		}

		for(int i = 0; i < volumeSliders.Length; i++)
		{
			var target = (GameSettings.VolumeTarget)i;
			if (volumeSliders[i] != null)
				volumeSliders[i].onValueChanged.AddListener(value => GameSettings.SetVolume(target, value));
		}
	}

	private void OnEnable()
	{
		if(firstSelected)
			firstSelected.Select();

		for (int i = 0; i < volumeSliders.Length; i++)
		{
			var target = (GameSettings.VolumeTarget)i;
			if (volumeSliders[i] != null)
				volumeSliders[i].value = GameSettings.GetVolume(target);
		}
	}

	void UpdateResolution()
	{
		// Unity itself remembers resolution settings, so we only need the Ui for it (no need to save in GameSettings)
		Resolution res = resolutions[resolutionDropdown.value];
		Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
	}
}
