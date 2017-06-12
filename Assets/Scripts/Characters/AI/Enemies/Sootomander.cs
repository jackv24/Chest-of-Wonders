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
        Sequence followTarget = new Sequence();

        Sequence targetInRange = new Sequence();
        targetInRange.behaviours.Add(new GetTarget("Player"));
        targetInRange.behaviours.Add(new Patrol(patrolRange, minPatrolDistance, transform.position.x, minMoveInterval, maxMoveInterval));
        targetInRange.behaviours.Add(new CheckRange(aggroRange, true));

        Selector turnToPlayer = new Selector();
        turnToPlayer.behaviours.Add(new CheckFacingTarget());

        Sequence turnAfterDistance = new Sequence();
        turnAfterDistance.behaviours.Add(new InvertResult(new CheckRange(turnStopRange)));
        turnAfterDistance.behaviours.Add(new StopFaceTarget());

        turnToPlayer.behaviours.Add(turnAfterDistance);

        followTarget.behaviours.Add(targetInRange);
        followTarget.behaviours.Add(turnToPlayer);
        followTarget.behaviours.Add(new WalkTowards());

        behaviour = followTarget;
    }

    private void Start()
    {
        ConstructBehaviour();

        startPos = transform.position;
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
