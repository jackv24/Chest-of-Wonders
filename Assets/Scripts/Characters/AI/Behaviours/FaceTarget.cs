using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class FaceTarget : IBehaviour
    {
        public FaceTarget()
        {
        }

        public Result Execute(AIAgent agent)
        {
            //Can only execute if there is a target
            if (agent.target)
            {
                //Get target and self x pos
                float targetPos = agent.target.position.x;
                float selfPos = agent.transform.position.x;

                //Direction towards target
                float dist = targetPos - selfPos;

                //Face towards target
                if(agent.targetDirection != (int)Mathf.Sign(dist))
                {
                    agent.targetDirection = (int)Mathf.Sign(dist);
                }

                return Result.Success;
            }

            return Result.Failure;
        }
    }
}
