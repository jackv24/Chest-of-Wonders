using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CharacterStats : MonoBehaviour, IDamageable
{
    public delegate void DeathEvent();
    public DeathEvent OnDeath;

	public delegate void DamagedEvent();
    public DamagedEvent OnDamaged;

    public int currentHealth = 100;
    public int maxHealth = 100;

    public ElementManager.Element element;

	[Space()]
	public SoundEventType hurtSound;
	public SoundEventType deathSound;

    [Space()]
	public bool damageImmunity = false;
	public float damageImmunityTime = 0.5f;
    public Vector2 damageTextOffset = Vector2.up;

    [Space()]
    public SpriteRenderer graphic;
    [Range(0, 1f)]
    public float flashAmount = 0.75f;
    public float flashInterval = 0.1f;

    [Space()]
    public float deathTime = 1f;
    public GameObject deathParticlePrefab;
    public Vector2 deathParticleOffset;

    [Space()]
    public GameObject deathDrop;
    public Vector2 dropOffset = Vector2.up;
    [Space()]
    public GameObject healthDrop;
    public int minHealthDrops = 0;
    public int maxHealthDrops = 2;
    public float healthDropForce = 10.0f;

    //If health is zero or below, character is dead
    public bool IsDead { get { return currentHealth <= 0; } }
	[HideInInspector]
	public bool hasDied = false;

	[Space()]
	public EnemyJournalRecord enemyRecord;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;
    private Blackboard blackboard;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
        blackboard = GetComponent<Blackboard>();
    }

    private void OnEnable()
    {
        if(graphic)
            graphic.material.SetFloat("_FlashAmount", 0);

        damageImmunity = false;

		hasDied = false;
    }

    //Removes the specified amount of health
    public bool RemoveHealth(int amount, int effectiveness)
    {
        //Do not remove health if immune to damage
        if (damageImmunity)
            return false;

        currentHealth -= amount;

        if (DamageText.instance)
            DamageText.instance.ShowDamageText((Vector2)transform.position + damageTextOffset, amount, effectiveness);

        //Keep health above or equal to 0
        if (currentHealth <= 0)
        {
			if(!hasDied)
				Die();

			currentHealth = 0;
		}
        else if (graphic && gameObject.activeSelf)
        {
            //Run only one stunned coroutine
            StopCoroutine("DamageFlash");
            StartCoroutine("DamageFlash", damageImmunityTime);
        }

		hurtSound.Play(transform.position);

        if (OnDamaged != null)
            OnDamaged();

        //Health was removed, so return true
        return true;
    }

    public bool RemoveHealth(int amount)
    {
        return RemoveHealth(amount, 0);
    }

    public bool TakeDamage(DamageProperties damageProperties)
    {
        int newAmount = ElementManager.CalculateDamage(damageProperties.amount, damageProperties.sourceElement, element);

        //Return new calculated health with the effectiveness
        return RemoveHealth(newAmount, newAmount != damageProperties.amount ? (newAmount > damageProperties.amount ? 1 : -1) : 0);
    }

    public bool AddHealth(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            return true;
        }
        else
            return false;
    }

    public void Die()
    {
		hasDied = true;

		enemyRecord?.RecordKill();

        //Show stunned flashing character
        if(graphic)
            StartCoroutine("DamageFlash", deathTime);
        //Count down to death
        StartCoroutine("DeathTimer", deathTime);

        if (characterMove)
        {
            characterMove.canMove = false;
            characterMove.Velocity = Vector2.zero;
        }

        if (characterAnimator)
            characterAnimator.SetStunned(true);
    }

    IEnumerator DeathTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        if(deathParticlePrefab)
        {
            GameObject obj = ObjectPooler.GetPooledObject(deathParticlePrefab);
            obj.transform.position = transform.position + (Vector3)deathParticleOffset;
        }

        if (OnDeath != null)
            OnDeath();

        if(graphic)
            graphic.material.SetFloat("_FlashAmount", 0);

        if (deathDrop)
        {
            GameObject dropped = ObjectPooler.GetPooledObject(deathDrop);
            dropped.transform.position = (Vector2)gameObject.transform.position + dropOffset;

			//If a soul was dropped, set it's element to this character's element
			SoulContainer soul = dropped.GetComponent<SoulContainer>();
			if (soul)
				soul.element = element;
        }

        if(healthDrop)
        {
            int dropAmount = Random.Range(minHealthDrops, maxHealthDrops + 1);

            for(int i = 0; i < dropAmount; i++)
            {
                GameObject drop = ObjectPooler.GetPooledObject(healthDrop);
                drop.transform.position = (Vector2)gameObject.transform.position + dropOffset;

                Rigidbody2D body = drop.GetComponent<Rigidbody2D>();
                body.velocity = Vector2.zero;
                body.angularVelocity = 0;

                Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f));
                dir.Normalize();

                body.AddForce(dir * healthDropForce, ForceMode2D.Impulse);
            }
        }

		deathSound.Play(transform.position);

        if ((characterAnimator && !characterAnimator.Death()) || !characterAnimator)
            gameObject.SetActive(false);
    }

    IEnumerator DamageFlash(float duration)
    {
        float elapsed = 0;

        Material mat = graphic.material;

        damageImmunity = true;

        while(elapsed < duration)
        {
            //Flash off
            mat.SetFloat("_FlashAmount", 0);
            yield return new WaitForSeconds(flashInterval);

            //Flash on
            mat.SetFloat("_FlashAmount", flashAmount);
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        mat.SetFloat("_FlashAmount", 0);

        damageImmunity = false;
    }

    public void SetAggro(bool value)
    {
        if(blackboard)
        {
            blackboard.SetValue("aggro", true);
        }
    }
}
