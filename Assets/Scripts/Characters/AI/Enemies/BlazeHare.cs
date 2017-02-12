using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class BlazeHare : AIAgent
{
    public float aggroRange = 5f;

    public float stopRange = 1f;

    public float hopAnticipation = 1f;
    public bool controlInAir = false;

    public override void ConstructBehaviour()
    {
        //TO-DO: Create some kind of system for creating and loading behaviour trees
        GameObject player = GameObject.FindWithTag("Player");

        Selector moveTo = new Selector();

        Sequence walkIfOutsideRange = new Sequence();

        CheckRange stop = new CheckRange(player.transform, stopRange, true, false);
        //WalkTowards walk = new WalkTowards(player.transform);
        HopTowards hop = new HopTowards(player.transform, hopAnticipation, controlInAir);

        walkIfOutsideRange.behaviours.Add(new InvertResult(stop));
        walkIfOutsideRange.behaviours.Add(hop);

        Sequence returnToIdle = new Sequence();
        returnToIdle.behaviours.Add(new InvertResult(new CheckRange(player.transform, aggroRange)));
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
