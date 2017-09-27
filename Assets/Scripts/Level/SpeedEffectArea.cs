using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffectArea : MonoBehaviour
{
	public float multiplier = 0.75f;

	//Keep a dictionary of initial speed values to return later (slow down multiple characters)
	private Dictionary<CharacterMove, float> speedBook = new Dictionary<CharacterMove, float>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		CharacterMove move = collision.GetComponent<CharacterMove>();

		if(move)
		{
			//Add initial speed to dictionary for restoring later
			speedBook.Add(move, move.moveSpeed);

			//Set new move speed by multiplier
			move.moveSpeed *= multiplier;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		CharacterMove move = collision.GetComponent<CharacterMove>();

		if (move)
		{
			//If this character is already entered (should be)
			if (speedBook.ContainsKey(move))
			{
				//Restore move speed
				move.moveSpeed = speedBook[move];

				//Remove from dictionary
				speedBook.Remove(move);
			}
		}
	}
}
