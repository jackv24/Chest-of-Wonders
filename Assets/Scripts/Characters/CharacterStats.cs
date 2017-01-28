using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int currentHealth = 100;
    public int maxHealth = 100;

    [Space()]
    public int currentMana = 100;
    public int maxMana = 100;

    [Space()]
    public Vector2 damageTextOffset = Vector2.up;

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
    }

    //Removes the specified amount of health
    public void RemoveMana(int amount)
    {
        currentMana -= amount;

        //Keep mana above or equal to 0
        if (currentMana <= 0)
            currentMana = 0;
    }

    public void Die()
    {
        //TODO: Animate death and then disable
        gameObject.SetActive(false);
    }
}
