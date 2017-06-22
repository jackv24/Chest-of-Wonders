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

        public float cooldown = 0;

        public Attack(int attackID)
        {
            this.attackID = attackID;
        }

        public Attack(int attackID, float cooldown)
        {
            this.attackID = attackID;
            this.cooldown = cooldown;
        }

        public Result Execute(AIAgent agent)
        {
            if (agent.endAttack && agent.currentAttack == attackID)
            {
                agent.endAttack = false;
                agent.attacking = false;

                //Debug.Log("Ending attack " + attackID);

                return Result.Success;
            }

            if (agent.attackCooldown <= 0 && !agent.attacking && agent.currentAttack != attackID)
            {
                agent.attackCooldown = cooldown;

                //Start attack
                agent.Attack(attackID);

                //Debug.Log("Starting attack " + attackID);

                //Start pending
                return Result.Pending;
            }

            //Continue pending while attacking
            if (agent.attacking && agent.currentAttack == attackID)
            {
                //Debug.Log("Pending attack " + attackID);

                return Result.Pending;
            }

            //Attack could not be performed
            return Result.Failure;
        }
    }
}
