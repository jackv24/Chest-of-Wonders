﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [Header("Character Scripts")]
    public CharacterMove characterMove;
    public CharacterStats characterStats;
    public PlayerAttack playerAttack;

    [Header("Behaviour Values")]
    public float slideTime = 0.5f;
    public AnimationCurve slideCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1,0));
    private bool canSlide = false;

    [Space()]
    public GameObject slideEffect;
	public GameObject pushblockPuff;

    [Header("Attacks")]
    public GameObject batSwingCollider;
    public SoundEffectBase.SoundEffect batSwingSound;
    [Space()]
    public GameObject chargedBatSwingCollider;
    public SoundEffectBase.SoundEffect chargedBatSwingSound;
    [Space()]
    public GameObject downstrikeCollider;
    public SoundEffectBase.SoundEffect downstrikeSound;
    public float downstrikeFallSpeed = 10.0f;
    public float hitGroundScreenShake = 1.0f;
    public float downstrikeEndImmunity = 0.25f;
	[Space()]
	public GameObject[] magicAttackColliders;

    private SoundEffectBase soundEffects;

    void Awake()
    {
        soundEffects = GetComponent<SoundEffectBase>();
    }

    private void Start()
    {
        if (characterStats)
        {
            characterStats.OnDamaged += DisableColliders;

            if (playerAttack)
                characterStats.OnDamaged += delegate { playerAttack.canAttack = true; };
        }

        DisableColliders();
    }

    void DisableColliders()
    {
        if (batSwingCollider)
            batSwingCollider.SetActive(false);

        if (chargedBatSwingCollider)
            chargedBatSwingCollider.SetActive(false);

        if (downstrikeCollider)
            downstrikeCollider.SetActive(false);

		foreach(GameObject obj in magicAttackColliders)
		{
			obj.SetActive(false);
		}
    }

    public void AllowMovement()
    {
        if (characterMove)
        {
            canSlide = false;
            characterMove.canMove = true;
        }

        if(playerAttack)
            playerAttack.canAttack = true;
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

        playerAttack.canAttack = false;

		float beforeGroundedTime = Time.time;

        while (!characterMove.isGrounded)
            yield return new WaitForEndOfFrame();

		//Keep track of time taken in air before grounded
		float afterGroundedTime = Time.time;
		float waitElapsed = afterGroundedTime - beforeGroundedTime;

		slideTime -= waitElapsed;

		//Don't do slide if there is no time left
		if (slideTime <= 0)
			yield break;

        characterMove.canMove = false;

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

    public void EnableChargedBatCollider()
    {
        if (chargedBatSwingCollider)
            chargedBatSwingCollider.SetActive(true);
    }
    public void DisableChargedBatCollider()
    {
        if (chargedBatSwingCollider)
            chargedBatSwingCollider.SetActive(false);
    }
    public void PlayChargedBatSwingSound()
    {
        if (soundEffects && chargedBatSwingSound.clip)
        {
            soundEffects.PlaySound(chargedBatSwingSound);
        }
    }

    public void StartDownStrike(float fallDelay)
    {
        StartCoroutine("Downstrike", fallDelay);
    }
    public void PlayDownstrikeSound()
    {
        if (soundEffects && downstrikeSound.clip)
        {
            soundEffects.PlaySound(downstrikeSound);
        }
    }

	public void EnableMagicAttackCollider(int index)
	{
		if(index < magicAttackColliders.Length)
		{
			if(magicAttackColliders[index])
			{
				magicAttackColliders[index].SetActive(true);
			}
		}
	}
	public void DisableMagicAttackCollider(int index)
	{
		if (index < magicAttackColliders.Length)
		{
			if (magicAttackColliders[index])
			{
				magicAttackColliders[index].SetActive(false);
			}
		}
	}

	IEnumerator Downstrike(float fallDelay)
    {
        //Player can not control during downstrike
        characterMove.canMove = false;
        playerAttack.canAttack = false;

        //Stop movement and cache gravity
        float initialGravity = characterMove.gravity;
        characterMove.gravity = 0;
        characterMove.velocity = Vector2.zero;

        //Can not be hurt during attack
        characterStats.damageImmunity = true;

        //Wait for swing animation
        yield return new WaitForSeconds(fallDelay);

        //Enable the bat collider
        if (downstrikeCollider)
            downstrikeCollider.SetActive(true);

        while (!characterMove.isGrounded)
        {
            //Move downwards at constant speed until grounded
            characterMove.velocity.y = -downstrikeFallSpeed;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        //Downwards screen shake
        Vector2 camOffset = new Vector2(0, -hitGroundScreenShake);
        Camera.main.transform.position += (Vector3)camOffset;

        //Disable collider
        if (downstrikeCollider)
            downstrikeCollider.SetActive(false);

        //Restore gravity and control
        characterMove.gravity = initialGravity;
        characterMove.canMove = true;
        playerAttack.canAttack = true;

        yield return new WaitForSeconds(downstrikeEndImmunity);

        //Player can be hurt again
        characterStats.damageImmunity = false;
    }

	public void SpawnPuff()
	{
		if(pushblockPuff)
		{
			GameObject obj = ObjectPooler.GetPooledObject(pushblockPuff);
			obj.transform.position = transform.position;

			Vector3 scale = obj.transform.localScale;
			scale.x = Mathf.Sign(characterMove.velocity.x);
			obj.transform.localScale = scale;
		}
	}
}
