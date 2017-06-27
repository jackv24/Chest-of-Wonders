using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public AIAgent agent;

    public CharacterStats characterStats;
    public CharacterMove characterMove;

    [Header("Attacks")]
    public GameObject[] attackColliders;

    [Space()]
    public Transform mouthTransform;
    public GameObject ashBall;

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

    public void AshBreath()
    {
        GameObject obj = ObjectPooler.GetPooledObject(ashBall);
        obj.transform.position = mouthTransform.position;

        Projectile proj = obj.GetComponent<Projectile>();
		proj.SetOwner(transform.parent.gameObject);
		proj.Fire(new Vector2(-transform.localScale.x, 0));

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

        characterMove.canMove = true;
    }
}
