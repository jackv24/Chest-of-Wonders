using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Executes a list of behaviours in order until one of them succeeds, returning success. Returns failure if none succeed.
    /// </summary>
    public class Selector : IBehaviour
    {
        public List<IBehaviour> behaviours = new List<IBehaviour>();

        private IBehaviour pendingBehaviour = null;

        public Result Execute(AIAgent agent)
        {
            int index = 0;

            //If there is a behaviour pending
            if (pendingBehaviour != null)
            {
                //Start index at that behaviour and clear pending
                index = behaviours.IndexOf(pendingBehaviour);
                pendingBehaviour = null;
            }

            for (int i = index; i < behaviours.Count; i++)
            {
                Result r = behaviours[i].Execute(agent);

                if (r == Result.Success)
                    return Result.Success;
                else if (r == Result.Pending)
                {
                    //Set pending behaviour and return pending
                    pendingBehaviour = behaviours[i];
                    return Result.Pending;
                }
            }

            return Result.Failure;
        }
    }
}
