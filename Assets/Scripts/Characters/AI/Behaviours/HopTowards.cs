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

        public float anticipationTime;
        private float nextHopTime = 0;

        private bool controlInAir = true;
        private float lastDirection;

        private bool startedAnticipating = false;

        public HopTowards(Transform target)
        {
            this.target = target;
        }

        public HopTowards(Transform target, float anticipationTime)
        {
            this.target = target;
            this.anticipationTime = anticipationTime;
        }

        public HopTowards(Transform target, float anticipationTime, bool controlInAir)
        {
            this.target = target;
            this.anticipationTime = anticipationTime;
            this.controlInAir = controlInAir;
        }

        public Result Execute(AIAgent agent)
        {
            //Get direction to target
            Vector2 direction = target.position - agent.transform.position;
            direction.Normalize();

            //Direction should always be 1 or -1
            float xInput = direction.x >= 0 ? 1 : -1;

            CharacterMove move = agent.characterMove;
            CharacterAnimator anim = agent.characterAnimator;

            if(move)
            {
                Result result = Result.Success;

                //If character is not on the ground
                if (!move.isGrounded)
                {
                    //Set direction to move (if air contro, is disabled keep moving in same direction)
                    move.Move(controlInAir ? xInput : lastDirection);

                    //Don't change behaviours if air control is not allowed
                    if (!controlInAir)
                        result = Result.Pending;

                    //Release jump button
                    move.Jump(false);
                }
                //Hop after anticipation time
                else if (Time.time >= nextHopTime)
                {
                    nextHopTime = Time.time + anticipationTime;
                    startedAnticipating = false;

                    //Set direction to move for ait control
                    lastDirection = xInput;

                    //Hold jump button
                    move.Jump(true);
                }
                //When on ground and waiting to hop, don't move
                else
                {
                    move.Move(0);

                    if(!startedAnticipating && move.isGrounded)
                    {
                        anim.animator.SetTrigger("anticipate");
                        startedAnticipating = true;
                    }
                }

                //Should always succeed unless there is no movement script
                return result;
            }

            //There was no movement script, so it failed
            return Result.Failure;
        }
    }
}
