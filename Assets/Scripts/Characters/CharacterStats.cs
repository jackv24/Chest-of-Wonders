using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int currentHealth = 100;
    public int maxHealth = 100;

    [Space()]
    public Vector2 damageTextOffset = Vector2.up;

    [Space()]
    public SpriteRenderer graphic;
    [Range(0, 1f)]
    public float flashAmount = 0.75f;
    public float flashInterval = 0.1f;
    public float flashDuration = 0.5f;

    [Space()]
    public float deathTime = 1f;
    public GameObject deathParticlePrefab;
    public Vector2 deathParticleOffset;

    //If health is zero or below, character is dead
    public bool IsDead { get { return currentHealth <= 0; } }

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    //Removes the specified amount of health
    public void RemoveHealth(int amount)
    {
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
            StopCoroutine("Stunned");
            StartCoroutine("Stunned", flashDuration);
        }
    }

    public void Die()
    {
        //Show stunned flashing character
        StartCoroutine("Stunned", deathTime);
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

        //TODO: Handle actual death and respawn
        gameObject.SetActive(false);
    }

    IEnumerator Stunned(float duration)
    {
        float elapsed = 0;

        Material mat = graphic.material;

        while(elapsed < duration)
        {
            //Flash on
            yield return new WaitForSeconds(flashInterval);
            mat.SetFloat("_FlashAmount", flashAmount);

            //Flash off
            yield return new WaitForSeconds(flashInterval);
            mat.SetFloat("_FlashAmount", 0);

            elapsed += flashInterval * 2;
        }
    }
}
