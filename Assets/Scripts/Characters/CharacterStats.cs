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

    //Removes the specified amount of health
    public void RemoveHealth(int amount)
    {
        currentHealth -= amount;

        //Keep health above or equal to 0
        if (currentHealth <= 0)
            currentHealth = 0;
    }

    //Removes the specified amount of health
    public void RemoveMana(int amount)
    {
        currentMana -= amount;

        //Keep mana above or equal to 0
        if (currentMana <= 0)
            currentMana = 0;
    }
}
