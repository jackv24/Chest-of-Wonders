using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Executes a random behaviour
    /// </summary>
    public class Randomiser : IBehaviour
    {
        public List<IBehaviour> behaviours = new List<IBehaviour>();

        private IBehaviour pendingBehaviour = null;

        public Result Execute(AIAgent agent)
        {
            //If there is a behaviour pending
            if (pendingBehaviour != null)
            {
                //Start index at that behaviour and clear pending
                Result r = pendingBehaviour.Execute(agent);

                if (r != Result.Pending)
                {
                    pendingBehaviour = null;

                    return r;
                }

                return r;
            }
            else
            {
                int index = Random.Range(0, behaviours.Count);

                return behaviours[index].Execute(agent);
            }
        }
    }
}
