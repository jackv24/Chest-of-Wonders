﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
	public AudioClip clip;

	private void Start()
	{
		MusicManager musicManager = MusicManager.Instance;

		//Switch to this music clip when level starts
		if(musicManager || clip)
			musicManager.SwitchTo(clip);
	}
}