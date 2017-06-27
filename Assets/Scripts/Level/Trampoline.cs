using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
	public float jumpMultiplier = 4.0f;

	private float initialJumpForce = 0;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			CharacterMove move = collision.gameObject.GetComponent<CharacterMove>();

			if (move)
			{
				initialJumpForce = move.jumpForce;

				move.jumpForce = initialJumpForce * jumpMultiplier;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			CharacterMove move = collision.gameObject.GetComponent<CharacterMove>();

			if (move)
			{
				StartCoroutine(ReturnForceAmount(move));
			}
		}
	}

	IEnumerator ReturnForceAmount(CharacterMove move)
	{
		while (move.velocity.y > 0)
		{
			yield return new WaitForEndOfFrame();
		}

		move.jumpForce = initialJumpForce;
	}
}
