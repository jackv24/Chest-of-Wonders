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
            StartCoroutine("SlideStopOverTime", slideTime);
        }
    }

    IEnumerator SlideStopOverTime(float slideTime)
    {
        while (!characterMove.IsGrounded)
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
                //Change velocity to fit curve (scaled)
                vel.x = initialMoveSpeed * slideCurve.Evaluate(timeElapsed / slideTime);
                vel.y = body.velocity.y;

                body.velocity = vel;

                yield return new WaitForEndOfFrame();
                timeElapsed += Time.deltaTime;
            }
        }
    }
}
