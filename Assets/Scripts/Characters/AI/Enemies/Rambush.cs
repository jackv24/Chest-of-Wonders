﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Rambush : AIAgent
{
    public float aggroRange = 5f;

    public float stopRange = 1f;

    public override void ConstructBehaviour()
    {
        Selector moveTo = new Selector();

        Sequence walkIfOutsideRange = new Sequence();

        CheckRange stop = new CheckRange(stopRange, true, false);
        //WalkTowards walk = new WalkTowards(player.transform);
        WalkTowards walk = new WalkTowards();

        walkIfOutsideRange.behaviours.Add(new InvertResult(stop));
        walkIfOutsideRange.behaviours.Add(walk);

        Sequence returnToIdle = new Sequence();
        returnToIdle.behaviours.Add(new GetTarget("Player"));
        returnToIdle.behaviours.Add(new InvertResult(new CheckRange(aggroRange)));
        returnToIdle.behaviours.Add(new StopMovement());

        moveTo.behaviours.Add(returnToIdle);
        moveTo.behaviours.Add(walkIfOutsideRange);
        moveTo.behaviours.Add(new StopMovement());

        behaviour = moveTo;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-stopRange, 1) + (Vector2)transform.position, new Vector2(-stopRange, -1) + (Vector2)transform.position);
        Gizmos.DrawLine(new Vector2(stopRange, 1) + (Vector2)transform.position, new Vector2(stopRange, -1) + (Vector2)transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
