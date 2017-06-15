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
    public float attack2Range = 2.5f;

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
        attack1.behaviours.Add(new Attack(1));

        Sequence attack2 = new Sequence();
        attack1.behaviours.Add(new CheckRange(attack2Range, true, true, false));
        attack1.behaviours.Add(new Attack(2));

        Selector attackOrMove = new Selector();
        attackOrMove.behaviours.Add(attack1);
        attackOrMove.behaviours.Add(attack2);

        b.behaviours.Add(idle);
        b.behaviours.Add(attackOrMove);
        b.behaviours.Add(new WalkTowards());

        behaviour = b;
    }

    private void Start()
    {
        startPos = transform.position;

        ConstructBehaviour();
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

        characterAnimator.animator.SetTrigger("Attack " + index);
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
