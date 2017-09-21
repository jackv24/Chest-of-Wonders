using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulContainer : MonoBehaviour
{
	public ElementManager.Element element;

	[Space()]
	public float restTime = 2.0f;
	public float moveSpeed = 10.0f;
	public float absorbedRange = 0.25f;

	private static PlayerMagicBank bank;
	private static Transform absorbPoint;

	private Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		//Make sure rigidbody is enabled after object pooling
		body.isKinematic = false;

		//First instance will find and set static value for bank
		if (!bank)
		{
			GameObject obj = GameObject.FindWithTag("Player");

			if (obj)
				bank = obj.GetComponent<PlayerMagicBank>();
		}

		if (bank)
		{
			StartCoroutine(Absorb());
		}
	}

	IEnumerator Absorb()
	{
		//Wait after spawning before being absorbed
		yield return new WaitForSeconds(2.0f);

		//First instance finds transform point to absorb to (player's hand)
		if(!absorbPoint)
		{
			PlayerAttack attack = bank.GetComponent<PlayerAttack>();

			if (attack)
				absorbPoint = attack.magicAbsorbPoint;
		}

		if (absorbPoint)
		{
			//Disable physics
			body.isKinematic = true;

			//Loop until close enough to absorb point
			float distance = float.MaxValue;
			while (distance > absorbedRange)
			{
				//Lerp towards absorb point
				Vector3 pos = transform.position;
				pos = Vector3.Lerp(pos, absorbPoint.position, moveSpeed * Time.deltaTime);
				pos.z = transform.position.z;
				transform.position = pos;

				//Calculate distance for looping check
				distance = Vector2.Distance(transform.position, absorbPoint.position);

				yield return new WaitForEndOfFrame();
			}
		}
		else
			Debug.LogWarning("No magic absorb point found!");

		//Add soul to bank
		bank.AddSoul(element);

		//Disable for object pooling
		gameObject.SetActive(false);
	}
}
