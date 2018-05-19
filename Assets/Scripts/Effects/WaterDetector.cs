using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDetector : MonoBehaviour
{
	private WaterPhysics waterPhysics;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!waterPhysics)
			waterPhysics = transform.parent.GetComponent<WaterPhysics>();

		if(waterPhysics)
		{
			CharacterMove move = collision.GetComponent<CharacterMove>();

			if (move)
				waterPhysics.Splash(transform.position.x, move.Velocity.y / 40f, WaterPhysics.SplashType.Push);
			else
			{
				Rigidbody2D body = collision.GetComponent<Rigidbody2D>();

				if (body)
					waterPhysics.Splash(transform.position.x, body.velocity.y * body.mass / 40f, WaterPhysics.SplashType.Push);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!waterPhysics)
			waterPhysics = transform.parent.GetComponent<WaterPhysics>();

		if (waterPhysics)
		{
			CharacterMove move = collision.GetComponent<CharacterMove>();

			if (move)
				waterPhysics.Splash(transform.position.x, move.Velocity.y / 40f, WaterPhysics.SplashType.Pull);
			else
			{
				Rigidbody2D body = collision.GetComponent<Rigidbody2D>();

				if (body)
					waterPhysics.Splash(transform.position.x, body.velocity.y * body.mass / 40f, WaterPhysics.SplashType.Pull);
			}
		}
	}
}
