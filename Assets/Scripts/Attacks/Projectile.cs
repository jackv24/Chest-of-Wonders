using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("The force to apply to the rigidbody when fired.")]
    public float shotForce = 10f;

    [Tooltip("How long this projectile can exist.")]
    public float lifeTime = 5f;

    [Space()]
    public bool destroyOnCollision = true;
    public GameObject explosionPrefab;

    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        //Reset values (object pooling)
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;
        body.rotation = 0;

        //Disable after its lifetime
        StartCoroutine("DisableAfterTime", lifeTime);
    }

    IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        SpawnEffect();
        gameObject.SetActive(false);
    }

    public void Fire(Vector2 direction)
    {
        //Set to face firection of fire
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        //Add initial force
        body.AddForce(direction * shotForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //If it should destroy on collision
        if (destroyOnCollision)
        {
            //Make sure the timer coroutine is no longer running
            StopCoroutine("DisableAfterTime");

            //Return to pool
            SpawnEffect();
            gameObject.SetActive(false);
        }
    }

    public void SpawnEffect()
    {
        //If there is an explosion prefab
        if (explosionPrefab)
        {
            //Get from pool
            GameObject effect = ObjectPooler.GetPooledObject(explosionPrefab);

            //Position at projectile position
            effect.transform.position = transform.position;
        }
    }
}
