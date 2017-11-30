using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
    public class MoveCharacter : ActionTask<CharacterMove>
    {
        public BBParameter<float> direction;

        protected override string info
        {
            get
            {
                string dir = "Stop";

                if (direction.useBlackboard)
                {
                    dir = direction.isNone ? "<color=red>None</color>" : direction.name;
                }
                else
                {
                    if (direction.value > 0)
                        dir = "Right";
                    else if (direction.value < 0)
                        dir = "Left";
                }

                return string.Format("{0} (<b>{1}</b>)", base.info, dir);
            }
        }

        protected override void OnExecute()
        {
            if (!direction.isNone)
            {
                agent.Move(direction.value);
            }

            EndAction(true);
        }
    }
}
