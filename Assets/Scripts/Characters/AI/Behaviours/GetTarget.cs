using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class GetTarget : IBehaviour
    {
        public string targetTag;

        public GetTarget(string tag)
        {
            targetTag = tag;
        }

        public Result Execute(AIAgent agent)
        {
            if(!agent.target)
            {
                GameObject t = GameObject.FindWithTag(targetTag);

                if(t)
                {
                    agent.target = t.transform;
                    return Result.Success;
                }

                return Result.Failure;
            }

            return Result.Success;
        }
    }
}
