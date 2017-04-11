using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Using the CharacterMove script attached to the Agent, moves the agent along the ground
    /// </summary>
    public class WalkTowards : IBehaviour
    {
        public WalkTowards()
        {
        }

        public Result Execute(AIAgent agent)
        {
            if (agent.target)
            {
                //Get direction to target
                Vector2 direction = agent.target.position - agent.transform.position;
                direction.Normalize();

                //Direction should always be 1 or -1
                float xInput = direction.x >= 0 ? 1 : -1;

                CharacterMove move = agent.characterMove;

                if (move)
                {
                    //Set direction to move
                    move.Move(xInput);

                    //Should always succeed unless there is no movement script
                    return Result.Success;
                }
            }

            //There was no movement script, so it failed
            return Result.Failure;
        }
    }
}
