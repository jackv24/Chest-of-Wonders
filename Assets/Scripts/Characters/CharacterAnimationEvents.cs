using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public AIAgent agent;

    public CharacterStats characterStats;

    private void Start()
    {
        if (characterStats)
            characterStats.OnDamaged += EndAttack;
    }

    public void StartAttack()
    {
        if(characterStats)
            characterStats.damageImmunity = true;
    }

    public void EndAttack()
    {
        if (agent)
        {
            agent.attacking = false;
            agent.currentAttack = -1;

            agent.endAttack = true;
        }

        if (characterStats)
            characterStats.damageImmunity = false;
    }
}
