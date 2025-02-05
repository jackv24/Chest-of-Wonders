﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
	public float jumpMultiplier = 4.0f;

	private float initialJumpForce = 0;
	private float initialGravity = 0;
	private bool jumped = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player" && jumped == false)
		{
			CharacterMove move = collision.gameObject.GetComponent<CharacterMove>();

			if (move && collision.transform.position.y > transform.position.y)
			{
				initialJumpForce = move.jumpForce;
				initialGravity = move.gravity;

				move.jumpForce = initialJumpForce * jumpMultiplier;
				move.gravity = initialGravity * jumpMultiplier;
				jumped = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player" && jumped == true)
		{
			CharacterMove move = collision.gameObject.GetComponent<CharacterMove>();

			if (move && initialJumpForce > 0)
			{
				StartCoroutine(ReturnForceAmount(move));
			}
		}
	}

	IEnumerator ReturnForceAmount(CharacterMove move)
	{
		while (move.Velocity.y > 0)
		{
			yield return new WaitForEndOfFrame();
		}

		move.jumpForce = initialJumpForce;
		move.gravity = initialGravity;
		jumped = false;
	}
}
