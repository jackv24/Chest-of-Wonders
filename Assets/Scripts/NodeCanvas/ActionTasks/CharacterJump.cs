using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Character")]
    public class CharacterJump : ActionTask<CharacterMove>
    {
        public BBParameter<float> jumpDirection = 0;

        public BBParameter<float> moveSpeed = 1;
        private float oldMoveSpeed;

        public BBParameter<float> holdJumpTime = 0.5f;
        public BBParameter<bool> waitUntilLand = true;

        private float jumpEndTime;
        private bool holdingJump = false;

        private bool jumpMoved = false;

        private bool waitFrame = false;

        protected override string info
        {
            get
            {
				if (!agent)
					return base.info;

                string dir = "<color=grey>Up</color>";

                if (jumpDirection.value > 0)
                    dir = "<b>Right</b>";
                else if (jumpDirection.value < 0)
                    dir = "<b>Left</b>";

                return string.Format(
                    "{0} ({1}s, {2}, {3}m/s)",
                    base.info,
                    holdJumpTime.isNone ? agent.jumpTime.ToString("<color=grey>0.0</color>") : holdJumpTime.value.ToString("<b>0.0</b>"),
                    dir,
                    moveSpeed.isNone ? agent.moveSpeed.ToString("<color=grey>0.0</color>") : moveSpeed.value.ToString("<b>0.0</b>"));
            }
        }

        protected override void OnExecute()
        {
            agent.Jump(true);
            holdingJump = true;

            jumpEndTime = Time.time + (holdJumpTime.isNone ? agent.jumpTime : holdJumpTime.value);

            if (!jumpDirection.isNone && jumpDirection.value != 0)
            {
                if (!moveSpeed.isNone)
                {
                    oldMoveSpeed = agent.moveSpeed;
                    agent.moveSpeed = moveSpeed.value;
                }

                agent.Move(jumpDirection.value);
                jumpMoved = true;
            }

            waitFrame = true;
        }

        protected override void OnUpdate()
        {
            if (waitFrame)
                waitFrame = false;
            else
            {
                if (holdingJump && Time.time >= jumpEndTime)
                {
                    agent.Jump(false);
                    holdingJump = false;
                }

                if (waitUntilLand.value)
                {
                    if (agent.IsGrounded && agent.Velocity.y <= 0)
                        EndAction(true);
                }
                else
                    EndAction(true);
            }
        }

        protected override void OnStop()
        {
            if (!moveSpeed.isNone)
                agent.moveSpeed = oldMoveSpeed;

            if (jumpMoved)
                agent.Move(0);
        }
    }
}
