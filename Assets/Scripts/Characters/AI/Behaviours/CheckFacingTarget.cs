using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class CheckFacingTarget : IBehaviour
    {
        bool defaultRight = false;

        CharacterAnimator anim = null;

        public CheckFacingTarget()
        {
        }

        public CheckFacingTarget(bool defaultRight)
        {
            this.defaultRight = defaultRight;
        }

        public Result Execute(AIAgent agent)
        {
            //Can only execute if there is a target
            if (agent.target)
            {
                if (anim == null)
                    anim = agent.gameObject.GetComponent<CharacterAnimator>();

                //Get target and self x pos
                float targetPos = agent.target.position.x;
                float selfPos = agent.transform.position.x;

                //Get direction agent is facing
                float facing = anim.animator.transform.localScale.x * (defaultRight ? 1 : -1);

                //Direction towards target
                float dist = targetPos - selfPos;

                //If facing same direction as target, return succes, otherwise false
                return Mathf.Sign(facing) == Mathf.Sign(dist) ? Result.Success : Result.Failure;
            }

            return Result.Failure;
        }
    }
}
