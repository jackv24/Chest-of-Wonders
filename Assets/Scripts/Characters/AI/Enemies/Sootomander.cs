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

    private Vector2 startPos;

    public override void ConstructBehaviour()
    {
        Sequence b = new Sequence();

        b.behaviours.Add(new GetTarget("Player"));
        b.behaviours.Add(new Patrol(patrolRange, minPatrolDistance, startPos.x, minMoveInterval, maxMoveInterval));
        b.behaviours.Add(new CheckRange(aggroRange, true, true, true));
        b.behaviours.Add(new WalkTowards());

        behaviour = b;
    }

    private void Start()
    {
        startPos = transform.position;

        ConstructBehaviour();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-turnStopRange, 1) + (Vector2)transform.position, new Vector2(-turnStopRange, -1) + (Vector2)transform.position);
        Gizmos.DrawLine(new Vector2(turnStopRange, 1) + (Vector2)transform.position, new Vector2(turnStopRange, -1) + (Vector2)transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        float patrol = patrolRange / 2;
        Vector2 pos = Application.isPlaying ? startPos : (Vector2)transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(-patrol, 1) + pos, new Vector2(-patrol, -1) + pos);
        Gizmos.DrawLine(new Vector2(patrol, 1) + pos, new Vector2(patrol, -1) + pos);
    }
}
