using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeCustom
{
    /// <summary>
    /// Returns success if the target is within the specified range
    /// </summary>
    public class StopFaceTarget : IBehaviour
    {
        private bool hasStopped = false;

        private bool shouldTurn = false;
        private bool canStop = true;

        private Animator anim;

        public GameObject slideEffect;
        private GameObject currentSlideEffect;
        private bool startedSlideEffect = false;

        public StopFaceTarget()
        {
        }

        public StopFaceTarget(GameObject slideEffect)
        {
            this.slideEffect = slideEffect;
        }

        public Result Execute(AIAgent agent)
        {
            if (!anim)
                anim = agent.characterAnimator.animator;

            //Can only execute if there is a target
            if (agent.target)
            {
                //Get target and self x pos
                float targetPos = agent.target.position.x;
                float selfPos = agent.transform.position.x;

                //Direction towards target
                float dist = targetPos - selfPos;

                //Face towards target
                if(agent.targetDirection != (int)Mathf.Sign(dist) && hasStopped && shouldTurn == false)
                {
                    shouldTurn = true;

                    if (agent.characterMove)
                        agent.characterMove.Move(0);

                    if (anim)
                    {
                        anim.SetBool("turning", true);
                    }
                }

                if (!hasStopped)
                {
                    anim.SetBool("stopping", true);

                    if (agent.characterMove)
                        agent.characterMove.Move(0);

                    if (Mathf.Abs(agent.characterMove.velocity.x) < agent.characterMove.moveSpeed * 0.1f && canStop)
                    {
                        canStop = false;
                        hasStopped = true;

                        anim.SetBool("stopping", false);
                    }

                    if(currentSlideEffect)
                        currentSlideEffect.transform.position = agent.transform.position;

                    if (!startedSlideEffect)
                    {
                        startedSlideEffect = true;

                        //Show slide effect
                        if (slideEffect)
                        {
                            currentSlideEffect = ObjectPooler.GetPooledObject(slideEffect);

                            ParticleSystem system = currentSlideEffect.GetComponentInChildren<ParticleSystem>();
                            system.Play(true);
                        }
                    }

                    return Result.Pending;
                }
                else
                {
                    shouldTurn = false;
                    canStop = true;
                    hasStopped = false;

                    startedSlideEffect = false;

                    if (currentSlideEffect)
                    {
                        ParticleSystem system = currentSlideEffect.GetComponentInChildren<ParticleSystem>();
                        system.Stop();
                        currentSlideEffect = null;
                    }

                    if (anim)
                        anim.SetBool("turning", false);

                    return Result.Success;
                }
            }

            return Result.Failure;
        }
    }
}
