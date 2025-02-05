﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sootie : MonoBehaviour
{
    public Transform target;

    private GameObject graphic;
    private float oldDirection = 0;

    [Space()]
    public float moveSpeed = 5.0f;
    public bool defaultRight = false;

	public float followRange = 5.0f;
	public bool mustBeBelow = true;
    private bool follow = false;

    [Space()]
    public LayerMask deathCollisionLayer;

    private Rigidbody2D body;
    private CharacterStats characterStats;

	private Transform player;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        if(!target)
        {
            target = GameManager.instance.player.transform;
        }

        if(target)
        {
            SpriteRenderer rend = GetComponentInChildren<SpriteRenderer>();

            if (rend)
                graphic = rend.gameObject;
        }

		player = GameManager.instance.player.transform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If collided with ground
        if(((1 << collision.collider.gameObject.layer) & deathCollisionLayer) > 0)
        {
            if (characterStats)
                characterStats.Die();
            else
                gameObject.SetActive(false);
        }
    }

	public void SetAggro()
	{
		body.isKinematic = false;
		follow = true;
	}

    private void Update()
    {
        if(target)
        {
            if (!follow && (Vector3.Distance(transform.position, player.position) > followRange || (player.position.y > transform.position.y && mustBeBelow)))
            {
                body.isKinematic = true;
                body.velocity = Vector2.zero;

                return;
            }
            else
            {
				body.isKinematic = false;
				follow = true;
			}

            Vector2 direction = (target.position - transform.position).normalized;

            Vector2 velocity = body.velocity;
            velocity.x = direction.x * moveSpeed;
            body.velocity = velocity;

            if(Mathf.Sign(direction.x) != oldDirection)
            {
                oldDirection = Mathf.Sign(direction.x);

                Vector3 scale = graphic.transform.localScale;
                scale.x = oldDirection * (defaultRight ? 1 : -1);
                graphic.transform.localScale = scale;
            }
        }
    }

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, followRange);
	}
}
