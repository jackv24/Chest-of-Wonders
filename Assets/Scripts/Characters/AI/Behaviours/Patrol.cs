using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class Patrol : IBehaviour
    {
        private float range = 5.0f;

        private float minMoveInterval = 1.0f;
        private float maxMoveInterval = 3.0f;
        private float nextMoveTime = 0;

        private float minMoveDistance = 2.0f;

        private float minX;
        private float maxX;
        private float startPos;

        private float targetX = 0;
        private float moveDir = 0;

        private bool moving = false;

        public Patrol(float range, float minMoveDistance, float startPos, float minMoveInterval, float maxMoveInterval)
        {
            this.range = range;
            this.minMoveDistance = minMoveDistance;

            minX = startPos - range / 2;
            maxX = startPos + range / 2;
            this.startPos = startPos;

            this.minMoveInterval = minMoveInterval;
            this.maxMoveInterval = maxMoveInterval;
        }

        public Result Execute(AIAgent agent)
        {
            //Only patrol until aggro'd
            if (agent.aggro == false)
            {
                //Only patrol after a random interval
                if (Time.time >= nextMoveTime && !moving)
                {
                    float multiplier = agent.transform.position.x > startPos ? -1 : 1;

                    //Move a random amount (direction depends on side of patrol)
                    float moveAmount = Random.Range(minMoveDistance, range) * multiplier;

                    //Calculate target position
                    targetX = agent.transform.position.x + moveAmount;

                    //Clamp target position to within patrol range
                    targetX = Mathf.Clamp(targetX, minX, maxX);

                    //Calculate direction to move
                    moveDir = Mathf.Sign(targetX - agent.transform.position.x);

                    moving = true;
                }

                if (moving)
                {
                    //If the agent is not close to it's target
                    if (Mathf.Abs(targetX - agent.transform.position.x) > 0.1f)
                    {
                        //Move towards target
                        agent.characterMove.Move(moveDir);
                    }
                    else
                    {
                        //If agent has reached target, stop moving
                        moving = false;
                        agent.characterMove.Move(0);

                        //Calculate next move time
                        nextMoveTime = Time.time + Random.Range(minMoveInterval, maxMoveInterval);
                    }
                }
            }

            //Always successful
            return Result.Success;
        }
    }
}
