using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class OptionsUI : MonoBehaviour
{
	public GameObject firstSelected;
	private GameObject previousSelected;

	public bool skipFirstTime = true;
	private bool firstTime = true;

	[Space()]
	public Dropdown resolutionDropdown;
	public Toggle fullscreenToggle;

	private List<Resolution> resolutions;

	public Toggle postFXToggle;

	[System.Serializable]
	public class VolumeSlider
	{
		public Slider slider;
		public string key = "Volume";

		private AudioMixer mixer;

		public void Setup(AudioMixer audioMixer)
		{
			mixer = audioMixer;

			if(slider)
			{
				slider.value = PlayerPrefs.GetFloat(key, 1.0f);

				slider.onValueChanged.AddListener(UpdateMixer);
				UpdateMixer(slider.value);
			}
		}

		void UpdateMixer(float value)
		{
			if(mixer)
				mixer.SetFloat(key, LinearToDecibel(value));
		}

		public void SavePrefs()
		{
			if(slider)
				PlayerPrefs.SetFloat(key, slider.value);
		}

		public static float LinearToDecibel(float linear)
		{
			float dB;

			if (linear != 0)
				dB = 20.0f * Mathf.Log10(linear);
			else
				dB = -144.0f;

			return dB;
		}
	}

	public AudioMixer audioMixer;
	public VolumeSlider masterVolume;
	public VolumeSlider musicVolume;
	public VolumeSlider soundVolume;

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

		if(postFXToggle)
		{
			postFXToggle.isOn = PlayerPrefs.GetInt("PostFX", 1) > 0 ? true : false;

			var postFX = FindObjectOfType<UnityEngine.PostProcessing.PostProcessingBehaviour>();

			if (postFX)
			{
				postFXToggle.onValueChanged.AddListener((isOn) => { postFX.enabled = isOn; });
				postFX.enabled = postFXToggle.isOn;
			}
		}

		masterVolume.Setup(audioMixer);
		musicVolume.Setup(audioMixer);
		soundVolume.Setup(audioMixer);
	}

	private void OnEnable()
	{
		if (skipFirstTime && firstTime)
		{
			firstTime = false;
			return;
		}

		if(firstSelected)
		{
			previousSelected = EventSystem.current.currentSelectedGameObject;

			EventSystem.current.firstSelectedGameObject = firstSelected;
			EventSystem.current.SetSelectedGameObject(firstSelected);
		}
	}

	private void OnDisable()
	{
		if (postFXToggle)
			PlayerPrefs.SetInt("PostFX", postFXToggle.isOn ? 1 : 0);

		masterVolume.SavePrefs();
		musicVolume.SavePrefs();
		soundVolume.SavePrefs();

		if(previousSelected)
		{
			EventSystem.current.firstSelectedGameObject = previousSelected;
			EventSystem.current.SetSelectedGameObject(previousSelected);
		}
	}

	void UpdateResolution()
	{
		Resolution res = resolutions[resolutionDropdown.value];

		Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
	}
}
