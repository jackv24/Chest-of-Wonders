using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public delegate void NormalEvent();
    public NormalEvent OnDeath;

    public int currentHealth = 100;
    public int maxHealth = 100;

    public ElementManager.Element element;

    [Space()]
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

    //If health is zero or below, character is dead
    public bool IsDead { get { return currentHealth <= 0; } }

    private bool damageImmunity = false;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;
    private CharacterSound characterSound;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
        characterSound = GetComponent<CharacterSound>();
    }

    private void OnEnable()
    {
        graphic.material.SetFloat("_FlashAmount", 0);

        damageImmunity = false;
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
            currentHealth = 0;

            Die();
        }
        else if (graphic && gameObject.activeSelf)
        {
            //Run only one stunned coroutine
            StopCoroutine("DamageFlash");
            StartCoroutine("DamageFlash", damageImmunityTime);
        }

        if (characterSound)
            characterSound.PlaySound(characterSound.hurtSound);

        //Health was removed, so return true
        return true;
    }

    public bool RemoveHealth(int amount)
    {
        return RemoveHealth(amount, 0);
    }

    public bool RemoveHealth(int amount, ElementManager.Element sourceElement)
    {
        int newAmount = ElementManager.CalculateDamage(amount, sourceElement, element);

        //Return new calulcated health with the effectiveness
        return RemoveHealth(newAmount, newAmount != amount ? (newAmount > amount ? 1 : -1) : 0);
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

        if (deathDrop)
        {
            GameObject dropped = ObjectPooler.GetPooledObject(deathDrop);
            dropped.transform.position = (Vector2)gameObject.transform.position + dropOffset;
        }

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
}
