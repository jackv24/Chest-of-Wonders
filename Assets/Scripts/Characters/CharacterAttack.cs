using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    public Attack primaryAttack;
    public Attack secondaryAttack;

    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    //Easy use function to use the current primary and secondary attacks (called from input class)
    public void UsePrimary()
    {
        UseAttack(primaryAttack);
    }
    public void UseSecondary()
    {
        UseAttack(secondaryAttack);
    }

    //Attempts to use a specific attack (if there is enough mana)
    void UseAttack(Attack attack)
    {
        if (GameManager.instance.canMove)
        {
            //If there is a characterStats attached, attacks must use mana
            if (characterStats)
            {
                //If there is enough mana...
                if (characterStats.currentMana >= attack.manaCost)
                {
                    //...remove amount of mana for attack, and use the attack
                    characterStats.RemoveMana(attack.manaCost);
                    attack.Use();
                }
                //If there is not enough mana do not attack
                else
                    Debug.Log("Not enough mana");
            }
            //If not, just use the attack
            else
                attack.Use();
        }
    }
}
