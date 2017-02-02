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

        public Result Execute(AIAgent agent)
        {
            for(int i = 0; i < behaviours.Count; i++)
            {
                if (behaviours[i].Execute(agent) == Result.Failure)
                    return Result.Failure;
            }

            return Result.Success;
        }
    }
}
