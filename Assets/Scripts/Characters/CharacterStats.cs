using System;
using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour, IDamageable
{
	// Events (use System.Action for NodeCanvas compatibility)
    public event Action OnDeath;
    public event Action OnDamaged;
	public event Action OnKnockbackRecover;

    [SerializeField]
    protected int currentHealth = 100;
    public int CurrentHealth { get { return currentHealth; } }

    [SerializeField]
    protected int maxHealth = 100;
    public int MaxHealth { get { return maxHealth; } }

    [SerializeField]
    private ElementManager.Element element;

	[Header("Taking Damage")]
    [SerializeField]
    private bool damageImmunity = false;
    public bool DamageImmunity
    {
        get { return damageImmunity; }
        set { damageImmunity = value; }
    }

    [SerializeField]
    private float damageImmunityTime = 0.5f;

    [SerializeField]
    private Vector2 damageTextOffset = Vector2.up;

	[Space, SerializeField]
    private SoundEventType hurtSound;

    [SerializeField]
    private CameraShakeTarget hurtCameraShake;

    [SerializeField]
    private GameObject hurtEffect;

    [SerializeField]
    private Vector2 hurtEffectOffset;

	[Space, SerializeField]
    private SpriteRenderer graphic;
    public SpriteRenderer Graphic { get { return graphic; } }

    [Range(0, 1f), SerializeField]
    private float flashAmount = 0.75f;

    [SerializeField]
    private float flashInterval = 0.1f;

	[Space, SerializeField]
    protected float knockbackDistance = 0.5f;

    [SerializeField]
    protected float knockbackTime = 0.1f;

    [Header("Death")]
    [SerializeField]
    private float deathTime = 1f;

    [SerializeField]
    private GameObject deathParticlePrefab;

    [SerializeField]
    private Vector2 deathParticleOffset;

	[Space, SerializeField]
    private SoundEventType deathSound;

    [SerializeField]
    private CameraShakeTarget deathCameraShake;

	[Space, SerializeField]
    private GameObject deathDrop;

    [SerializeField]
    private Vector2 dropOffset = Vector2.up;

    [Space, SerializeField]
    private GameObject healthDrop;

    [SerializeField]
    private int minHealthDrops = 0;

    [SerializeField]
    private int maxHealthDrops = 2;

    [SerializeField]
    private float healthDropForce = 10.0f;

    //If health is zero or below, character is dead
    public bool IsDead { get { return currentHealth <= 0; } }

	private bool hasDied = false;

    protected virtual bool ShowDamageNumbers => true;

    [Space, SerializeField]
    private EnemyJournalRecord enemyRecord;

	private Coroutine damageFlashRoutine = null;
	private Coroutine knockBackRoutine = null;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;
    private Blackboard blackboard;

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        CustomGizmos.DrawPoint(dropOffset);
        CustomGizmos.DrawPoint(deathParticleOffset);
    }

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

    public bool TakeDamage(DamageProperties damageProperties)
    {
		int removeAmount = ElementManager.CalculateDamage(damageProperties.amount, damageProperties.sourceElement, element);

		//Attempt to remove health (might not work if invincible)
		if(RemoveHealth(removeAmount))
		{
			if (OnDamaged != null)
				OnDamaged();

            if (ShowDamageNumbers)
            {
                DamageText.ShowDamageText(
                    (Vector2)transform.position + damageTextOffset,
                    removeAmount,
                    ElementManager.GetEffectiveness(damageProperties.sourceElement, element)
                    );
            }

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

			hurtCameraShake.DoShake();

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

    //Removes the specified amount of health
    public bool RemoveHealth(int amount)
    {
        if (damageImmunity)
            return false;

        currentHealth -= amount;

        //Keep health above or equal to 0
        if (currentHealth <= 0)
        {
            if (!hasDied)
                Die();

            currentHealth = 0;
        }

        HealthUpdated();

        //Health was removed, so return true
        return true;
    }

    public bool AddHealth(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            HealthUpdated();

            return true;
        }
        else
            return false;
    }

    protected virtual void HealthUpdated() { }

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

        if (deathParticlePrefab)
            deathParticlePrefab.SpawnPooled(transform.TransformPoint(deathParticleOffset));

        if (OnDeath != null)
            OnDeath();

        if(graphic)
            graphic.material.SetFloat("_FlashAmount", 0);

        if (deathDrop)
        {
            GameObject dropped = deathDrop.SpawnPooled(transform.TransformPoint(dropOffset));

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
                GameObject drop = healthDrop.SpawnPooled(transform.TransformPoint(dropOffset));

                Rigidbody2D body = drop.GetComponent<Rigidbody2D>();
                body.velocity = Vector2.zero;
                body.angularVelocity = 0;

                Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f));
                dir.Normalize();

                body.AddForce(dir * healthDropForce, ForceMode2D.Impulse);
            }
        }

		deathSound.Play(transform.position);
		deathCameraShake.DoShake();

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
