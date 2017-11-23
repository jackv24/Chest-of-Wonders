using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeCustom;

public class AIAgent : MonoBehaviour
{
    public Transform target;
    public bool aggro = false;

    [HideInInspector] public int targetDirection = 0;
    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool endAttack = false;
    [HideInInspector] public int currentAttack = -1;
    [HideInInspector] public float attackCooldown = 0;

    [HideInInspector]
    public CharacterMove characterMove;
    [HideInInspector]
    public CharacterAnimator characterAnimator;
    [HideInInspector]
    public CharacterStats characterStats;

    protected IBehaviour behaviour;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
        characterStats = GetComponent<CharacterStats>();
    }

    void Start()
    {
        ConstructBehaviour();
    }

    public virtual void ConstructBehaviour()
    {
        //AIAgents that inherit from this class implement this function
    }

    void Update()
    {
        if(behaviour != null)
        {
            behaviour.Execute(this);
        }

        if(attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        else
            attackCooldown = 0;
    }

    public void SetAggro(bool value)
    {
        aggro = value;
    }

    public virtual void Attack(int index)
    {
        //AIAgents that inherit from this class implement this function

        attacking = true;
        currentAttack = index;
    }
}
