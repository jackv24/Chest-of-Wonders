using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class AIAgent : MonoBehaviour
{
    public Transform target;
    [HideInInspector]
    public int targetDirection = 0;

    [HideInInspector]
    public CharacterMove characterMove;
    [HideInInspector]
    public CharacterAnimator characterAnimator;

    protected IBehaviour behaviour;

    void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        characterAnimator = GetComponent<CharacterAnimator>();
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
    }
}
