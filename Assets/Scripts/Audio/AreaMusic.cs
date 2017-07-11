using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMusic : MonoBehaviour
{
	public AudioClip clip;
	private AudioClip oldClip;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//When player enters area, switch music
		if(collision.tag == "Player")
		{
			MusicManager musicManager = MusicManager.Instance;

			if(musicManager)
			{
				//Cache current music clip to switch back on exit
				oldClip = musicManager.primarySource.clip;

				musicManager.SwitchTo(clip);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		//When player exits area, switch back to old music
		if (collision.tag == "Player")
		{
			MusicManager musicManager = MusicManager.Instance;

			if (musicManager)
			{
				musicManager.SwitchTo(oldClip);
			}
		}
	}
}
