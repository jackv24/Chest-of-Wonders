using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    /// <summary>
    /// Executes a list of behaviours in order until one of them succeeds, returning success. Returns failure if none succeed.
    /// </summary>
    public class Selector : IBehaviour
    {
        public List<IBehaviour> behaviours = new List<IBehaviour>();

        public Result Execute(AIAgent agent)
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (behaviours[i].Execute(agent) == Result.Success)
                    return Result.Success;
            }

            return Result.Failure;
        }
    }
}
