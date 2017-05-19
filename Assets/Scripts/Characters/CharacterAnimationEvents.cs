using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    [Header("Character Scripts")]
    public CharacterMove characterMove;
    public CharacterSound characterSound;

    [Header("Behaviour Values")]
    public float slideTime = 0.5f;
    public AnimationCurve slideCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1,0));
    private bool canSlide = false;

    [Space()]
    public GameObject slideEffect;

    [Header("Attacks")]
    public GameObject batSwingCollider;
    public SoundEffectBase.SoundEffect batSwingSound;

    private SoundEffectBase soundEffects;

    void Awake()
    {
        soundEffects = GetComponent<SoundEffectBase>();
    }

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
        ParticleSystem system = null;

        if (slideEffect)
        {
            GameObject newSlideParticles = ObjectPooler.GetPooledObject(slideEffect);
            system = newSlideParticles.GetComponentInChildren<ParticleSystem>();
        }

        characterMove.canMove = false;

        while (!characterMove.isGrounded)
            yield return new WaitForEndOfFrame();

        if (canSlide)
        {
            //Get initial velocity
            Vector2 vel = characterMove.velocity;
            float initialMoveSpeed = vel.x;

            float timeElapsed = 0;

            //Slide over time
            while (timeElapsed <= slideTime)
            {
                if (system)
                {
                    //Only show particles when on ground
                    if (system.isStopped && characterMove.isGrounded)
                        system.Play();
                    else if (system.isPlaying && !characterMove.isGrounded)
                        system.Stop();
                }

                if (system)
                    system.transform.position = transform.position;

                //Change velocity to fit curve (scaled)
                vel.x = initialMoveSpeed * slideCurve.Evaluate(timeElapsed / slideTime);
                vel.y = characterMove.velocity.y;

                characterMove.velocity = vel;

                yield return new WaitForEndOfFrame();
                timeElapsed += Time.deltaTime;
            }

            if (system)
                system.Stop();
        }
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

    public void PlayBatSwingSound()
    {
        if(soundEffects && batSwingSound.clip)
        {
            soundEffects.PlaySound(batSwingSound);
        }
    }

    public void PlayFootstep()
    {
        if(characterSound)
        {
            characterSound.PlayFootstep();
        }
    }
}
