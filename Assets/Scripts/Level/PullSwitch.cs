﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullSwitch : MonoBehaviour
{
	[Tooltip("MUST BE UNIQUE (unless another door should be unlocked as well)")]
	public int uniqueID = 0;

	public float pullDistance = 1.0f;

	public float pullTime = 2.0f;
	public AnimationCurve pullCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	public Transform playerAttachPoint;

	[Space()]
	public Transform sparkSpawnPoint;
	public GameObject sparkEffect;
	public float sparkDelay = 0.0f;

	[Space()]
	public GameObject spawnObject;
	public Transform spawnPoint;
	private GameObject spawnedObject;

	private bool pulled = false;

	void Start()
	{
		//Check if this item has already been picked up
		if (SaveManager.instance.IsSwitchPulled(uniqueID))
		{
			pulled = true;

			transform.position += Vector3.down * pullDistance;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player" && !pulled)
		{
			pulled = true;

			StartCoroutine(PullOut(collision.gameObject));
		}
	}

	private void OnDisable()
	{
		//Make sure spawned object has been picked up
		if (pulled)
		{
			if (!spawnedObject || !spawnedObject.activeSelf)
			{
				SaveManager.instance.SetPulledSwitch(uniqueID);
			}
		}
	}

	IEnumerator PullOut(GameObject player)
	{
		CharacterMove characterMove = player.GetComponent<CharacterMove>();
		CharacterAnimator characterAnimator = player.GetComponent<CharacterAnimator>();

		float direction = Mathf.Sign(characterAnimator.animator.transform.localScale.x);

		//Stop enemies moving
		GameManager.instance.gameRunning = false;

		//Stop player from moving
		characterMove.scriptControl = false;
		characterMove.velocity = Vector2.zero;

		Vector3 initialPos = transform.localPosition;
		Vector3 targetPos = initialPos + Vector3.down * pullDistance;

		characterAnimator.animator.SetBool("pullSwitch", true);

		//Offset on x depending on the direction the player is facing
		Vector3 offset = playerAttachPoint.localPosition;
		offset.x *= direction;

		float elapsedTime = 0;
		bool spawnedSpark = false;

		while (elapsedTime <= pullTime)
		{
			//Lerp position using animation curve
			transform.localPosition = Vector3.Lerp(initialPos, targetPos, pullCurve.Evaluate(elapsedTime / pullTime));

			//Keep player at attach point
			player.transform.position = transform.position + offset;

			if(!spawnedSpark && elapsedTime >= sparkDelay)
			{
				spawnedSpark = true;

				if (sparkEffect)
				{
					GameObject obj = ObjectPooler.GetPooledObject(sparkEffect);
					obj.transform.position = sparkSpawnPoint.position;
				}
			}

			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}

		characterAnimator.animator.SetBool("pullSwitch", false);

		if(spawnPoint && spawnObject)
		{
			GameObject obj = Instantiate(spawnObject, spawnPoint);
			obj.transform.localPosition = Vector3.zero;

			obj.transform.parent = null;

			spawnedObject = obj;
		}

		//Restore game state
		GameManager.instance.gameRunning = true;
		characterMove.scriptControl = true;
	}
}