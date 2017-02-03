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

        public bool checkX = true;
        public bool checkY = true;

        public CheckRange(Transform target, float range)
        {
            this.target = target;
            this.range = range;
        }

        public CheckRange(Transform target, float range, bool checkX, bool checkY)
        {
            this.target = target;
            this.range = range;
            this.checkX = checkX;
            this.checkY = checkY;
        }

        public Result Execute(AIAgent agent)
        {
            //Calculate distance, and return success if in range, or failure if not
            Vector3 targetPos = target.position;

            //Set position to be the same as agent to "skip" desired checks
            if (!checkX)
                targetPos.x = agent.transform.position.x;
            if (!checkY)
                targetPos.y = agent.transform.position.y;

            return Vector2.Distance(agent.transform.position, targetPos) <= range ? Result.Success : Result.Failure;
        }
    }
}
