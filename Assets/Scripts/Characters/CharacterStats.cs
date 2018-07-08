using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class CharacterStats : MonoBehaviour, IDamageable
{
	//Events (use System.Action for NodeCanvas compatibility)
    public event System.Action OnDeath;
    public event System.Action OnDamaged;
	public event System.Action OnKnockbackRecover;

    public int currentHealth = 100;
    public int maxHealth = 100;

    public ElementManager.Element element;

	[Header("Taking Damage")]
	public bool damageImmunity = false;
	public float damageImmunityTime = 0.5f;
    public Vector2 damageTextOffset = Vector2.up;

	[Space()]
	public SoundEventType hurtSound;
	public CameraShake.ShakeType hurtCameraShake = CameraShake.ShakeType.EnemyHit;
	public GameObject hurtEffect;
	public Vector2 hurtEffectOffset;

	[Space()]
    public SpriteRenderer graphic;
    [Range(0, 1f)]
    public float flashAmount = 0.75f;
    public float flashInterval = 0.1f;

	[Space()]
	public float knockbackDistance = 0.5f;
	public float knockbackTime = 0.1f;

    [Header("Death")]
    public float deathTime = 1f;
    public GameObject deathParticlePrefab;
    public Vector2 deathParticleOffset;

	[Space()]
	public SoundEventType deathSound;
	public CameraShake.ShakeType deathCameraShake = CameraShake.ShakeType.EnemyKill;

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

	private Coroutine damageFlashRoutine = null;
	private Coroutine knockBackRoutine = null;

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
    public bool RemoveHealth(int amount)
    {
		if (damageImmunity)
			return false;

		currentHealth -= amount;

        //Keep health above or equal to 0
        if (currentHealth <= 0)
        {
			if(!hasDied)
				Die();

			currentHealth = 0;
		}

        //Health was removed, so return true
        return true;
    }

    public bool TakeDamage(DamageProperties damageProperties)
    {
		int removeAmount = ElementManager.CalculateDamage(damageProperties.amount, damageProperties.sourceElement, element);

		//Attempt to remove health (might not work if invincible)
		if(RemoveHealth(removeAmount))
		{
			if (OnDamaged != null)
				OnDamaged();

			DamageText.instance?.ShowDamageText(
				(Vector2)transform.position + damageTextOffset,
				removeAmount,
				DamageText.CalculateEffectiveness(damageProperties.amount, removeAmount)
				);

			//Do damage immunity flash only if this was not a fatal hit
			if (currentHealth > 0 && graphic && gameObject.activeSelf)
			{
				DoDamageFlash(damageImmunityTime);
			}

			hurtSound.Play(transform.position);

			if (hurtEffect)
			{
				GameObject obj = ObjectPooler.GetPooledObject(hurtEffect);
				obj.transform.SetPosition2D((Vector2)transform.position + hurtEffectOffset);

				CharacterHitMaskEffect maskEffect = obj.GetComponent<CharacterHitMaskEffect>();
				if (maskEffect)
				{
					maskEffect.SetOwner(this);
				}
			}

			CameraShake.Instance?.DoShake(hurtCameraShake);

			if (knockBackRoutine != null)
			{
				StopCoroutine(knockBackRoutine);
				OnKnockbackRecover?.Invoke();
			}
			StartCoroutine(Knockback(damageProperties));

			return true;
		}

        return false;
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

	private void DoDamageFlash(float time)
	{
		if (damageFlashRoutine != null)
			StopCoroutine(damageFlashRoutine);

		damageFlashRoutine = StartCoroutine(DamageFlash(time));
	}

    public void Die()
    {
		hasDied = true;

		enemyRecord?.RecordKill();

		//Show stunned flashing character
		DoDamageFlash(deathTime);

        //Count down to death
        StartCoroutine(DeathTimer(deathTime));

        if (characterMove)
        {
            characterMove.canMove = false;
            characterMove.Velocity = Vector2.zero;
        }

        characterAnimator?.SetStunned(true);
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
		CameraShake.Instance?.DoShake(deathCameraShake);

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

		damageFlashRoutine = null;
    }

    public void SetAggro(bool value)
    {
        if(blackboard)
        {
            blackboard.SetValue("aggro", true);
        }
    }

	protected virtual Vector2 GetKnockBackVelocity(DamageProperties damageProperties)
	{
		float speed = knockbackDistance / knockbackTime;

		return damageProperties.direction.normalized * speed;
	}

	private IEnumerator Knockback(DamageProperties damageProperties)
	{
		if (characterMove)
		{
			characterMove.MovementState = CharacterMovementStates.SetVelocity;
			characterMove.Velocity = GetKnockBackVelocity(damageProperties);

			yield return new WaitForSeconds(knockbackTime);

			characterMove.Velocity = Vector2.zero;
			characterMove.MovementState = CharacterMovementStates.Normal;
		}

		if(!IsDead)
			OnKnockbackRecover?.Invoke();

		knockBackRoutine = null;
	}
}
