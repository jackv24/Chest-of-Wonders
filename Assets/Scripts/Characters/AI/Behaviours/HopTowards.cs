using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Using the CharacterMove script attached to the Agent, moves the agent by jumping and only moving laterally once in the air
    /// </summary>
    public class HopTowards : IBehaviour
    {
        public Transform target;

        public HopTowards(Transform target)
        {
            this.target = target;
        }

        public Result Execute(AIAgent agent)
        {
            //Get direction to target
            Vector2 direction = target.position - agent.transform.position;
            direction.Normalize();

            //Direction should always be 1 or -1
            float xInput = direction.x >= 0 ? 1 : -1;

            CharacterMove move = agent.characterMove;

            if(move)
            {
                //If character is on the ground
                if (!move.isGrounded)
                {
                    //Release jump button
                    move.Jump(false);
                    //Set direction to move
                    move.Move(xInput);
                }
                else
                    //Hold jump button
                    move.Jump(true);

                //Should always succeed unless there is no movement script
                return Result.Success;
            }

            //There was no movement script, so it failed
            return Result.Failure;
        }
    }
}
