using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class Attack : IBehaviour
    {
        public int attackID;

        public Attack(int attackID)
        {
            this.attackID = attackID;
        }

        public Result Execute(AIAgent agent)
        {
            if(agent.endAttack)
            {
                agent.endAttack = false;

                return Result.Success;
            }

            if(!agent.attacking && agent.currentAttack != attackID)
            {
                //Start attack
                agent.Attack(attackID);

                //Start pending
                return Result.Pending;
            }

            //Continue pending while attacking
            if (agent.attacking && agent.currentAttack == attackID)
            {
                return Result.Pending;
            }

            //Attempting to attack does not fail. Attack is handled elsewhere
            return Result.Success;
        }
    }
}
