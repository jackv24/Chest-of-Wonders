using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFinishedAnim : MonoBehaviour
{
	public AnimationClip clip;

	public bool enableClearSprite = true;

	private SpriteRenderer sprite;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (clip)
			StartCoroutine(DisableAfterTime(clip.length));

		if (enableClearSprite && sprite)
			sprite.sprite = null;
	}

	IEnumerator DisableAfterTime(float time)
	{
		yield return new WaitForSeconds(time);

		gameObject.SetActive(false);
	}
}
