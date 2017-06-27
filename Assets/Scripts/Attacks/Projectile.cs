using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("The force to apply to the rigidbody when fired.")]
    public float shotForce = 10f;

    [Tooltip("How long this projectile can exist.")]
    public float lifeTime = 5f;

    [Space]
    public int damageAmount = 10;
    [TagSelector]
    public string damageTag;

    [Space()]
    public bool destroyOnCollision = true;
    public GameObject explosionPrefab;
    public AnimationClip deathAnimClip;
    public SoundEffectBase.SoundEffect deathSound;

    //Hidden from inspector as value is set by script when fired
    [HideInInspector]
    public ElementManager.Element element;

    private Rigidbody2D body;
    private GameObject owner;
    private Animator animator;
    private SoundEffectBase soundEffects;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        soundEffects = GameManager.instance.GetComponent<SoundEffectBase>();
    }

    void OnEnable()
    {
        //Reset values (object pooling)
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;
        body.rotation = 0;

        if (animator)
            animator.SetBool("isAlive", true);

        //Disable after its lifetime
        StartCoroutine("DisableAfterTime", lifeTime);

		ParticleSystem particles = GetComponentInChildren<ParticleSystem>();

		if(particles)
		{
			particles.Clear(true);
		}
    }

    IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Die();
    }

    public void SetOwner(GameObject obj)
    {
        owner = obj;

        //Ignore collision with owner (if there is one)
        if(owner)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.GetComponent<Collider2D>());
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
        bool hitCharacter = false;

        //If it hit something that should be damaged
        if(col.gameObject.tag == damageTag)
        {
            //Record hit to spawn effect later
            hitCharacter = true;

            //Get characterstats
            CharacterStats stats = col.gameObject.GetComponent<CharacterStats>();

            //If hit gameobject has characterstats
            if(stats)
            {
                //Apply damage
                stats.RemoveHealth(damageAmount, element);

                //Enemy aggro on hit
				if(col.collider.tag == "Enemy")
					col.gameObject.SendMessage("SetAggro", true);
            }
        }

        //If it should destroy on collision with ground, or character
        if (destroyOnCollision || hitCharacter)
        {
            //Make sure the timer coroutine is no longer running
            StopCoroutine("DisableAfterTime");

            //Return to pool
            Die();
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

    void Die()
    {
        StartCoroutine("DeathAnimation");
    }

    IEnumerator DeathAnimation()
    {
        if (animator)
        {
            animator.SetBool("isAlive", false);
        }

        if(soundEffects && deathSound.clip)
        {
            soundEffects.PlaySound(deathSound);
        }

        body.velocity = Vector2.zero;
        body.angularVelocity = 0;

        if (deathAnimClip)
            yield return new WaitForSeconds(deathAnimClip.length);

        SpawnEffect();

        gameObject.SetActive(false);
    }
}
