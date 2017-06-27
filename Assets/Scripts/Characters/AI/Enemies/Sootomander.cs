using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Sootomander : AIAgent
{
    public float aggroRange = 5f;
    public float turnStopRange = 1f;
    [Space]
    public bool defaultRight = false;

    [Header("Patrol")]
    public float patrolRange = 5.0f;
    public float minPatrolDistance = 2.0f;
    [Space()]
    public float minMoveInterval = 1.0f;
    public float maxMoveInterval = 3.0f;

    [Header("Attacking")]
    public float attack1Range = 2.0f;
    public float attack1Cooldown = 1.0f;
    [Space()]
    public float attack2Range = 2.5f;
    public float attack2Cooldown = 2.0f;
    [Space()]
    public float pounceReadyTime = 1.0f;
    public float pounceSpeed = 10.0f;
    public float pounceCooldown = 5.0f;
    private bool inPounce = false;
    [Space()]
    public float ashCooldown = 5.0f;

    [Header("Death")]
    public GameObject sootiePrefab;
    public int sootieAmount = 5;
    public float sootieSpawnHeight = 1.0f;
    public float sootieUpForce = 5.0f;
    public float sootieSpacing = 0.5f;

    private Vector2 startPos;

    public override void ConstructBehaviour()
    {
        Sequence b = new Sequence();

        Sequence idle = new Sequence();
        idle.behaviours.Add(new GetTarget("Player"));
        idle.behaviours.Add(new Patrol(patrolRange, minPatrolDistance, startPos.x, minMoveInterval, maxMoveInterval));
        idle.behaviours.Add(new CheckRange(aggroRange, true, true, true));

        Sequence attack1 = new Sequence();
        attack1.behaviours.Add(new CheckRange(attack1Range, true, true, false));
        attack1.behaviours.Add(new Attack(1, attack1Cooldown));

        Sequence attack2 = new Sequence();
        attack2.behaviours.Add(new CheckRange(attack2Range, true, true, false));
        attack2.behaviours.Add(new Attack(2, attack2Cooldown));

        Randomiser randomAttack = new Randomiser();
        randomAttack.behaviours.Add(new Attack(3, pounceCooldown));
        randomAttack.behaviours.Add(new Attack(4, ashCooldown));

        Selector attackOrMove = new Selector();
        attackOrMove.behaviours.Add(attack1);
        attackOrMove.behaviours.Add(attack2);
        attackOrMove.behaviours.Add(randomAttack);

        b.behaviours.Add(idle);
        b.behaviours.Add(new InvertResult(attackOrMove));
        b.behaviours.Add(new WalkTowards());

        behaviour = b;
    }

    private void Start()
    {
        startPos = transform.position;

        ConstructBehaviour();

        if (sootiePrefab)
        {
            characterStats.OnDeath += delegate
            {
                for(int i = 0; i < sootieAmount; i++)
                {
                    GameObject obj = ObjectPooler.GetPooledObject(sootiePrefab);
                    obj.transform.position = transform.position + Vector3.up * sootieSpawnHeight + new Vector3((-sootieAmount / (float)2) + i * sootieSpacing, 0);

                    Rigidbody2D body = obj.GetComponent<Rigidbody2D>();

                    if(body)
                    {
                        float force = Random.Range(0, sootieUpForce);

                        body.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    }
                }
            };
        }
    }

    void OnEnable()
    {
        BossInfoUI bossInfoUI = BossInfoUI.Instance;

        if (bossInfoUI)
            bossInfoUI.Show(characterStats);
    }

    void OnDisable()
    {
        BossInfoUI bossInfoUI = BossInfoUI.Instance;

        if (bossInfoUI)
            bossInfoUI.Hide();
    }

    public override void Attack(int index)
    {
        base.Attack(index);

        characterMove.Move(0);

        switch (index)
        {
            case 1:
            case 2:
                characterMove.canMove = false;
                characterAnimator.animator.SetTrigger("attack " + index);
                break;
            case 3:
                //Only one coroutine should run
                if(!inPounce)
                    StartCoroutine("Pounce");
                break;
            case 4:
                characterMove.canMove = false;
                characterAnimator.animator.SetTrigger("ashBreath");
                break;
        }
    }

    IEnumerator Pounce()
    {
        CharacterAnimationEvents animationEvents = GetComponentInChildren<CharacterAnimationEvents>();

        inPounce = true;

        //Debug.Log("Pounce Started!");

        float direction = Mathf.Sign(target.position.x - transform.position.x);

        characterAnimator.animator.SetBool("pounceReady", true);
        characterAnimator.animator.SetBool("pouncing", true);

        characterMove.Move(0);
        characterMove.canMove = false;

        yield return new WaitForSeconds(pounceReadyTime);

        characterMove.canMove = true;

        characterAnimator.animator.SetBool("pounceReady", false);

        float moveSpeed = characterMove.moveSpeed;

        characterMove.moveSpeed = pounceSpeed;
        characterMove.Jump(true);

        while(characterMove.velocity.y <= 0)
        {
            //Debug.Log("Waiting for jump");

            yield return new WaitForEndOfFrame();
        }

        while (!characterMove.isGrounded)
        {
            //Debug.Log("Waiting for ground...");
            characterMove.Move(direction);

            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("Landed!");

        characterMove.Jump(false);

        characterMove.moveSpeed = moveSpeed;
        characterMove.Move(0);

        inPounce = false;

        characterAnimator.animator.SetBool("pouncing", false);

        //Make sure attack is ended (may not have called end event because of interruptions)
        if (endAttack == false)
            animationEvents.EndAttack();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-turnStopRange, 1) + (Vector2)transform.position, new Vector2(-turnStopRange, -1) + (Vector2)transform.position);
        Gizmos.DrawLine(new Vector2(turnStopRange, 1) + (Vector2)transform.position, new Vector2(turnStopRange, -1) + (Vector2)transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attack1Range);
        Gizmos.DrawWireSphere(transform.position, attack2Range);

        float patrol = patrolRange / 2;
        Vector2 pos = Application.isPlaying ? startPos : (Vector2)transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(-patrol, 1) + pos, new Vector2(-patrol, -1) + pos);
        Gizmos.DrawLine(new Vector2(patrol, 1) + pos, new Vector2(patrol, -1) + pos);
    }
}
