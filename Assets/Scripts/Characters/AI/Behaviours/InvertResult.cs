using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// An effective NOT logic gate - inverts the result
    /// </summary>
    public class InvertResult : IBehaviour
    {
        //The behaviour to invert
        public IBehaviour behaviour;

        public InvertResult(IBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public Result Execute(AIAgent agent)
        {
            //Execute the behaviour and store the result
            Result r = behaviour.Execute(agent);

            //Return the opposite
            if (r == Result.Success)
                return Result.Failure;
            else if (r == Result.Failure)
                return Result.Success;
            //Pending has no opposite, so just return it
            else
                return Result.Pending;
        }
    }
}
