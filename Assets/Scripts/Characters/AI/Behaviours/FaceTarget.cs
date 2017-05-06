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
        public float turnTime = 0.25f;
        private float hasTurnedTime = 0;

        private bool shouldTurn = false;

        private Animator anim;

        public FaceTarget()
        {
        }

        public FaceTarget(float turnTime)
        {
            this.turnTime = turnTime;
        }

        public Result Execute(AIAgent agent)
        {
            if (!anim)
                anim = agent.characterAnimator.animator;

            //Can only execute if there is a target
            if (agent.target)
            {
                //Get target and self x pos
                float targetPos = agent.target.position.x;
                float selfPos = agent.transform.position.x;

                //Direction towards target
                float dist = targetPos - selfPos;

                //Face towards target
                if(agent.targetDirection != (int)Mathf.Sign(dist) && Time.time > hasTurnedTime && shouldTurn == false)
                {
                    shouldTurn = true;
                    hasTurnedTime = Time.time + turnTime;

                    if (agent.characterMove)
                        agent.characterMove.Move(0);

                    if (anim)
                        anim.SetBool("turning", true);
                }

                if (shouldTurn && Time.time < hasTurnedTime)
                {
                    return Result.Pending;
                }
                else
                {
                    shouldTurn = false;

                    if (anim)
                        anim.SetBool("turning", false);

                    return Result.Success;
                }
            }

            return Result.Failure;
        }
    }
}
