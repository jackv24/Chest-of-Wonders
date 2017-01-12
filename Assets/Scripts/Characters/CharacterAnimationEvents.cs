using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    [Header("Character Controllers")]
    public CharacterMove characterMove;

    [Header("Behaviour Values")]
    public float slideTime = 0.5f;
    public AnimationCurve slideCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1,0));
    private bool canSlide = false;

    [Space()]
    public ParticleSystem stepParticles;

    [Header("Attacks")]
    public GameObject batSwingCollider;

    private void Start()
    {
        if (batSwingCollider)
            batSwingCollider.SetActive(false);
    }

    public void AllowMovement()
    {
        if (characterMove)
        {
            canSlide = false;
            characterMove.canMove = true;
        }
    }

    public void DisallowMovement()
    {
        if (characterMove)
            characterMove.canMove = false;
    }

    public void SlideStopMovement()
    {
        if (characterMove)
        {
            canSlide = true;
            //Only play one at a time
            StopCoroutine("SlideStopOverTime");
            StartCoroutine("SlideStopOverTime", slideTime);
        }
    }

    IEnumerator SlideStopOverTime(float slideTime)
    {
        while (!characterMove.isGrounded)
            yield return new WaitForEndOfFrame();

        if (canSlide)
        {
            Rigidbody2D body = characterMove.body;

            //Get initial velocity
            float initialMoveSpeed = body.velocity.x;
            Vector2 vel = body.velocity;

            characterMove.canMove = false;

            float timeElapsed = 0;

            //Slide over time
            while (timeElapsed <= slideTime)
            {
                if (stepParticles)
                {
                    //Only show particles when on ground
                    if (stepParticles.isStopped && characterMove.isGrounded)
                        stepParticles.Play();
                    else if (stepParticles.isPlaying && !characterMove.isGrounded)
                        stepParticles.Stop();
                }

                //Change velocity to fit curve (scaled)
                vel.x = initialMoveSpeed * slideCurve.Evaluate(timeElapsed / slideTime);
                vel.y = body.velocity.y;

                body.velocity = vel;

                yield return new WaitForEndOfFrame();
                timeElapsed += Time.deltaTime;
            }

            if (stepParticles)
                stepParticles.Stop();
        }
    }

    public void EmitStepParticles(int amount)
    {
        if (stepParticles)
            stepParticles.Emit(amount);
    }

    public void EnableBatCollider()
    {
        if (batSwingCollider)
            batSwingCollider.SetActive(true);
    }

    public void DisableBatCollider()
    {
        if (batSwingCollider)
            batSwingCollider.SetActive(false);
    }
}
