using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance;

	public AudioSource primarySource;
	public AudioSource secondarySource;

	[Space()]
	public float fadeDuration = 1.0f;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//Primary source should play on start
		if (primarySource)
		{
			primarySource.volume = 1.0f;
			primarySource.enabled = true;
		}

		//Stop secondary source
		if (secondarySource)
		{
			secondarySource.volume = 0.0f;
			secondarySource.enabled = false;
		}
	}

	public void SwitchTo(AudioClip clip)
	{
		//Only one fade coroutine should run at a time
		StopCoroutine("FadeTo");
		StartCoroutine(FadeTo(clip, true));
	}

	IEnumerator FadeTo(AudioClip newClip, bool keepTime)
	{
		//No need to fade if the clip is the same
		if (primarySource.clip == newClip)
			yield return null;

		//Switch current clip to secondary source at max volume
		secondarySource.enabled = true;
		secondarySource.volume = 1.0f;
		secondarySource.clip = primarySource.clip;
		secondarySource.time = primarySource.time;

		secondarySource.Play();

		//Put new clip in primary source at min volume
		primarySource.volume = 0.0f;
		primarySource.clip = newClip;
		if(keepTime)
			primarySource.time = secondarySource.time;

		primarySource.Play();

		//Crossfade audio sources
		float elapsedTime = 0;
		while(elapsedTime < fadeDuration)
		{
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;

			primarySource.volume = Mathf.Lerp(0, 1.0f, elapsedTime / fadeDuration);
			secondarySource.volume = Mathf.Lerp(1.0f, 0, elapsedTime / fadeDuration);
		}

		//Disable secondary source after crossfade
		secondarySource.volume = 0;
		secondarySource.enabled = false;

		//Make sre primary source is at max volume
		primarySource.volume = 1.0f;
	}
}
