using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulContainer : MonoBehaviour
{
	public ElementManager.Element element;

	[Space()]
	public float restTime = 2.0f;
    public float moveSpeed = 5.0f;
    public float turnSpeed = 2.0f;
	public float turnAcceleration = 10.0f;
    public float absorbedRange = 0.25f;

    [Space()]
    public GameObject graphic;
    public GameObject absorbTrail;

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

        transform.SetRotationZ(0);

        if(graphic)
            graphic.SetActive(true);

		if(absorbTrail)
            absorbTrail.SetActive(false);

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
				absorbPoint = attack.soulAbsorbPoint;
		}

		if (absorbPoint)
		{
			//Disable physics
			body.isKinematic = true;

			if (graphic)
                graphic.SetActive(false);

            if (absorbTrail)
                absorbTrail.SetActive(true);

			//Set initial rotation to either -90
            transform.SetRotationZ(90);

            float currentTurnSpeed = turnSpeed;

            //Loop until close enough to absorb point
            float distance = float.MaxValue;
			while (distance > absorbedRange)
			{
                //Lerp towards absorb point
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);

                Vector3 toTarget = absorbPoint.position - transform.position;
                float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, currentTurnSpeed * Time.deltaTime);

                //Calculate distance for looping check
                distance = Vector2.Distance(transform.position, absorbPoint.position);

                currentTurnSpeed += turnAcceleration * Time.deltaTime;

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
