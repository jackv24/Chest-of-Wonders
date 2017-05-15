using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Rambush : AIAgent
{
    public float aggroRange = 5f;

    public float turnStopRange = 1f;

    public bool defaultRight = false;

    public GameObject slideEffect;

    public override void ConstructBehaviour()
    {
        //TODO: Fix, currently broken if turnStopRange is larger than aggroRange

        Sequence followTarget = new Sequence();

        Sequence targetInRange = new Sequence();
        targetInRange.behaviours.Add(new GetTarget("Player"));
        targetInRange.behaviours.Add(new CheckRange(aggroRange, true));

        Selector turnToPlayer = new Selector();
        turnToPlayer.behaviours.Add(new CheckFacingTarget());

        Sequence turnAfterDistance = new Sequence();
        turnAfterDistance.behaviours.Add(new InvertResult(new CheckRange(turnStopRange)));
        turnAfterDistance.behaviours.Add(new StopFaceTarget(slideEffect));

        turnToPlayer.behaviours.Add(turnAfterDistance);

        followTarget.behaviours.Add(targetInRange);
        followTarget.behaviours.Add(turnToPlayer);
        followTarget.behaviours.Add(new WalkTowards());

        behaviour = followTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-turnStopRange, 1) + (Vector2)transform.position, new Vector2(-turnStopRange, -1) + (Vector2)transform.position);
        Gizmos.DrawLine(new Vector2(turnStopRange, 1) + (Vector2)transform.position, new Vector2(turnStopRange, -1) + (Vector2)transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
