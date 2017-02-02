using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class CheckRange : IBehaviour
    {
        public Transform target;
        public float range;

        public CheckRange(Transform target, float range)
        {
            this.target = target;
            this.range = range;
        }

        public Result Execute(AIAgent agent)
        {
            //Calculate distance, and return success if in range, or failure if not
            return Vector2.Distance(agent.transform.position, target.position) <= range ? Result.Success : Result.Failure;
        }
    }
}
