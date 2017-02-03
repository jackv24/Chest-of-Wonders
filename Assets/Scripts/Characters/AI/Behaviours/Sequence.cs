using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    /// <summary>
    /// Executes a list of behaviours in order until one of them fails, returning failure. Returns success if none fail.
    /// </summary>
    public class Sequence : IBehaviour
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

            for(int i = index; i < behaviours.Count; i++)
            {
                Result r = behaviours[i].Execute(agent);

                if (r == Result.Failure)
                    return Result.Failure;
                else if (r == Result.Pending)
                {
                    //Set pending behaviour and return pending
                    pendingBehaviour = behaviours[i];
                    return Result.Pending;
                }
            }

            return Result.Success;
        }
    }
}
