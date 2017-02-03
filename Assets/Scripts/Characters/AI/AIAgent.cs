using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AIAgent : MonoBehaviour
{
    [HideInInspector]
    public CharacterMove characterMove;

    private IBehaviour behaviour;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
    }

    void Start()
    {
        //TO-DO: Create some kind of system for creating and loading behaviour trees
        GameObject player = GameObject.FindWithTag("Player");

        Selector moveTo = new Selector();

        Sequence walkIfOutsideRange = new Sequence();

        CheckRange range = new CheckRange(player.transform, 1f, true, false);
        //WalkTowards walk = new WalkTowards(player.transform);
        HopTowards hop = new HopTowards(player.transform);

        walkIfOutsideRange.behaviours.Add(new InvertResult(range));
        walkIfOutsideRange.behaviours.Add(hop);

        moveTo.behaviours.Add(walkIfOutsideRange);
        moveTo.behaviours.Add(new StopMovement());

        behaviour = moveTo;
    }

    void Update()
    {
        if(behaviour != null)
        {
            behaviour.Execute(this);
        }
    }
}
