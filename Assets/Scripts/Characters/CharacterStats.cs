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

        if (graphic && gameObject.activeSelf)
        {
            StopCoroutine("FlashSprite");
            StartCoroutine("FlashSprite", flashDuration);
        }
    }

    public void Die()
    {
        //TODO: Animate death and then disable
        gameObject.SetActive(false);
    }

    IEnumerator FlashSprite(float duration)
    {
        float elapsed = 0;

        Material mat = graphic.material;

        while(elapsed < duration)
        {
            yield return new WaitForSeconds(flashInterval);
            mat.SetFloat("_FlashAmount", flashAmount);

            yield return new WaitForSeconds(flashInterval);
            mat.SetFloat("_FlashAmount", 0);

            elapsed += flashInterval * 2;
        }
    }
}
