using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public delegate void NormalEvent();
    public NormalEvent OnDeath;

    public int currentHealth = 100;
    public int maxHealth = 100;

    [Space()]
    public float damageImmunityTime = 1.0f;
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

    //If health is zero or below, character is dead
    public bool IsDead { get { return currentHealth <= 0; } }

    private bool damageImmunity = false;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void OnEnable()
    {
        damageImmunity = false;
    }

    //Removes the specified amount of health
    public bool RemoveHealth(int amount)
    {
        //Do not remove health if immune to damage
        if (damageImmunity)
            return false;

        currentHealth -= amount;

        if (DamageText.instance)
            DamageText.instance.ShowDamageText((Vector2)transform.position + damageTextOffset, amount);

        //Keep health above or equal to 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            Die();
        }
        else if (graphic && gameObject.activeSelf)
        {
            //Run only one stunned coroutine
            StopCoroutine("DamageFlash");
            StartCoroutine("DamageFlash", damageImmunityTime);
        }

        //Health was removed, so return true
        return true;
    }

    public void Die()
    {
        //Show stunned flashing character
        StartCoroutine("DamageFlash", deathTime);
        //Count down to death
        StartCoroutine("DeathTimer", deathTime);

        if (characterMove)
            characterMove.canMove = false;

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

        graphic.material.SetFloat("_FlashAmount", 0);

        gameObject.SetActive(false);
    }

    IEnumerator DamageFlash(float duration)
    {
        float elapsed = 0;

        Material mat = graphic.material;

        damageImmunity = true;

        while(elapsed < duration)
        {
            //Flash on
            mat.SetFloat("_FlashAmount", flashAmount);
            yield return new WaitForSeconds(flashInterval);

            //Flash off
            mat.SetFloat("_FlashAmount", 0);
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        damageImmunity = false;
    }
}
