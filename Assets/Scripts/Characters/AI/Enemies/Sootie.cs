using System.Collections;
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

    public bool waitUntilOnScreen = true;
    private CameraFollow cam;

    [Space()]
    public LayerMask deathCollisionLayer;

    private Rigidbody2D body;
    private CharacterStats characterStats;

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

        cam = Camera.main.GetComponent<CameraFollow>();
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

    private void Update()
    {
        if(target)
        {
            if (waitUntilOnScreen && !cam.IsInView(transform.position))
            {
                body.isKinematic = true;
                body.velocity = Vector2.zero;

                return;
            }
            else
                body.isKinematic = false;

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
}
