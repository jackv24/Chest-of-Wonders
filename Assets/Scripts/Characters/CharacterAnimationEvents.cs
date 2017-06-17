using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public AIAgent agent;

    public CharacterStats characterStats;

    [Header("Attacks")]
    public GameObject[] attackColliders;

    private void Start()
    {
        if (characterStats)
            characterStats.OnDamaged += EndAttack;

        //Disable all attack colliders
        foreach (GameObject obj in attackColliders)
            obj.SetActive(false);
    }

    public void StartAttack(int attackIndex)
    {
        EndAttack();

        if(characterStats)
            characterStats.damageImmunity = true;

        //Enable attack collider if index is within range
        if (attackIndex < attackColliders.Length)
            attackColliders[attackIndex].SetActive(true);
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

        //Disable attack colliders
        foreach (GameObject obj in attackColliders)
            obj.SetActive(false);
    }
}
