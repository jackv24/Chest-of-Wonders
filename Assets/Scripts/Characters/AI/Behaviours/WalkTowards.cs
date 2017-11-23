﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeCustom
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
                float xInput = 0;

                if (agent.targetDirection != 0)
                {
                    xInput = agent.targetDirection;
                }
                else
                {
                    //Get direction to target
                    Vector2 direction = agent.target.position - agent.transform.position;
                    direction.Normalize();

                    //Direction should always be 1 or -1
                    xInput = direction.x >= 0 ? 1 : -1;
                }

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
