using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Calling Move on CharacterMove sets the velocity rather than moving it when called, so this behaviour exists to set the velocity back to zero
    /// </summary>
    public class StopMovement : IBehaviour
    {
        public Result Execute(AIAgent agent)
        {
            CharacterMove move = agent.characterMove;
            CharacterAnimator anim = agent.characterAnimator;

            if (move)
            {
                //Stop movement
                move.Move(0);
                
                //Should always succeed unless there is no movement script
                return Result.Success;
            }

            //There was no movement script, so it failed
            return Result.Failure;
        }
    }
}
