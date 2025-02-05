﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour
{
    [Serializable]
    public class ProjectileAttack
    {
        public GameObject projectilePrefab;
        public GameObject projectileCastEffectPrefab;

        public ElementManager.Element Element { get; private set; }

        public ProjectileAttack(ElementManager.Element element)
        {
            Element = element;
        }
    }

    public enum MagicProgression
    {
        Basic,
        Full
    }

    public event Action OnUpdateMagic;
    public event Action<ElementManager.Element> OnSwitchMagic;

    [Header("Magic")]
    public ProjectileAttack fireProjectile = new ProjectileAttack(ElementManager.Element.Fire);
    public ProjectileAttack grassProjectile = new ProjectileAttack(ElementManager.Element.Grass);
    public ProjectileAttack iceProjectile = new ProjectileAttack(ElementManager.Element.Ice);
    public ProjectileAttack windProjectile = new ProjectileAttack(ElementManager.Element.Wind);

    [Space()]
    public Transform upFirePoint;
    public Transform upForwardFirePoint;
    public Transform forwardFirePoint;
    public Transform downForwardFirePoint;
    public Transform downFirePoint;
    public Transform soulAbsorbPoint;

    [Space()]
    public float fireDelay = 0.25f;
    private float nextFireTime;

    [Space()]
    public GameObject failedCastEffect;
    public SoundEventType failedCastSound;

    [Space()]
    public float arrowDistance = 1.0f;
    public float arrowHeight = 0.5f;
    public Transform arrow;

    private Vector2 aimDirection;
    private int lastDiagonalDirection = 1;

    //Direction set by character move so that player doesn't have to hold x direction to fire
    private float directionX = 1;

    [Header("Bat Swing")]
    [Tooltip("The minimum amount of time the bat needs to charge before a damage multiplier is applied.")]
    public float chargeHoldTime = 1.0f;
    private float heldStartTime = 0;
    private bool isChargingBat;

    private Coroutine batChargeRoutine = null;

    [Space()]
    public SpriteRenderer graphic;
    public float flashAmount = 0.5f;
    public float flashInterval = 0.1f;

    private CharacterMove characterMove;
    private Animator animator;

    public bool IsHoldingBat { get; private set; }

    public bool CanAttack { get; private set; } = true;

    private MagicProgression _currentMagicProgression;
    public MagicProgression CurrentMagicProgression
    {
        get => _currentMagicProgression;
        set
        {
            _currentMagicProgression = value;
            UpdateMagic();
        }
    }

    public bool HasFireMagic { get; private set; }
    public bool HasGrassMagic { get; private set; }
    public bool HasIceMagic { get; private set; }
    public bool HasWindMagic { get; private set; }

    public ElementManager.Element SelectedElement { get; private set; }

    private void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
		animator = graphic.GetComponent<Animator>();
    }

    void Start()
    {
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
                CurrentMagicProgression = data.MagicProgression;

				SelectedElement = data.SelectedElement;

				HasFireMagic = data.HasFireMagic;
				HasGrassMagic = data.HasGrassMagic;
				HasIceMagic = data.HasIceMagic;
				HasWindMagic = data.HasWindMagic;

				UpdateMagic();
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				data.MagicProgression = CurrentMagicProgression;

				data.SelectedElement = SelectedElement;

				data.HasFireMagic = HasFireMagic;
				data.HasGrassMagic = HasGrassMagic;
				data.HasIceMagic = HasIceMagic;
				data.HasWindMagic = HasWindMagic;
			};
		}

		//Update magic UI to start
		UpdateMagic();

		//Create event handler to update the players facing direction
		if (characterMove)
            characterMove.OnChangedDirection += delegate (float newDir) { directionX = newDir; };
    }

    void OnEnable()
    {
        CanAttack = true;
    }

    public void UseMelee(bool buttonDown, Vector2 inputDirection)
    {
		StopBatCharge();

		if (CanAttack)
		{
			//Button pressed
			if(buttonDown && !IsHoldingBat)
			{
				heldStartTime = Time.time;
				batChargeRoutine = StartCoroutine(BatCharge(chargeHoldTime));

                // Force into correct facing direction (in case we're currently in a prevent turn animation)
                if (inputDirection.x != 0)
                    characterMove?.SetFacing(Mathf.Sign(inputDirection.x));

				animator?.SetTrigger("batSwing");
            }
			else if(!buttonDown && isChargingBat) //Button released
			{
                isChargingBat = false;

				//Play charged bat swing if fully charged, else let animator transition back to regular
				if (Time.time >= heldStartTime + chargeHoldTime)
					animator?.SetTrigger("batSwingCharged");
				else
					animator?.SetTrigger("batSwing");
            }
		}

		SetHoldingBat(buttonDown);
	}

	public void SetHoldingBat(bool value)
	{
		IsHoldingBat = value;

		animator?.SetBool("holdingBat", value);

		if(!value)
		{
			StopBatCharge();
		}
	}

	private void StopBatCharge()
	{
		if (batChargeRoutine != null)
		{
			StopCoroutine(batChargeRoutine);
			batChargeRoutine = null;

			//Set flash amount to 0 incase charge routine was stopped during a flash
			graphic?.material.SetFloat("_FlashAmount", 0);
		}
	}

    public void UpdateAimDirection(Vector2 direction, bool lockDiagonal)
    {
        //If desired, only allow firing in diagonal directions
        if (lockDiagonal)
        {
            if (direction.y == 0)
                direction.y = lastDiagonalDirection;
            else
            {
                direction.y = Mathf.Sign(direction.y);
                lastDiagonalDirection = (int)direction.y;
            }
        }

        //Make sure x direction is not zero unless firing straight up or down
        if (direction.x == 0 && (lockDiagonal || direction.y == 0))
            //Calculate facing direction based on forward fire point
            direction.x = directionX;

        //Snap direction to the nearest 45 degrees and normalize
        direction = Helper.SnapTo(direction, 45.0f);
        direction.Normalize();

        //Display diagonal lock arrow
        if (lockDiagonal)
        {
            arrow.gameObject.SetActive(true);

            arrow.transform.localPosition = new Vector2(0, arrowHeight) + direction;
            arrow.transform.localScale = new Vector3(Mathf.Sign(direction.x), Mathf.Sign(direction.y), 1);
        }
        else
            arrow.gameObject.SetActive(false);

        //Finally, set direction
        aimDirection = direction;
    }

    private void UpdateMagic()
    {
        OnUpdateMagic?.Invoke();
    }

	public void SwitchMagic(int direction)
	{
		if (CurrentMagicProgression <= MagicProgression.Basic)
			return;

		//Make sure direction is not larger than 1
		direction = (int)Mathf.Sign(direction);

		int selected = (int)SelectedElement;
		int max = System.Enum.GetNames(typeof(ElementManager.Element)).Length - 1;

		bool anyUnlocked = false;
		for (int i = 0; i < max; i++)
		{
			selected += direction;

			//Wrap around (skipping first element sinc it's none)
			if (selected <= 0) //Selected element can not be None
				selected = max;
			else if (selected > max)
				selected = 1;

			//Limit to unlocked magics
			bool success = false;
			switch((ElementManager.Element)selected)
			{
				case ElementManager.Element.Fire:
					if (HasFireMagic)
						success = true;
					break;
				case ElementManager.Element.Grass:
					if (HasGrassMagic)
						success = true;
					break;
				case ElementManager.Element.Ice:
					if (HasIceMagic)
						success = true;
					break;
				case ElementManager.Element.Wind:
					if (HasWindMagic)
						success = true;
					break;
			}

			if(success)
			{
				anyUnlocked = true;
				break;
			}
		}

		if (anyUnlocked)
			SetSelectedMagic((ElementManager.Element)selected);
	}

	public void SetSelectedMagic(ElementManager.Element element)
	{
		SelectedElement = element;

		OnSwitchMagic?.Invoke(SelectedElement);

		UpdateMagic();
	}

    public void UseProjectileMagic()
    {
		if (!CanAttack || CurrentMagicProgression <= MagicProgression.Basic)
			return;

		//If magic slot was chosen correctly, and there is an attack in the slot
		if (Time.time >= nextFireTime)
		{
			ProjectileAttack attackType = null;

			switch(SelectedElement)
			{
				case ElementManager.Element.Fire:
					attackType = fireProjectile;
					break;
				case ElementManager.Element.Ice:
					attackType = iceProjectile;
					break;
				case ElementManager.Element.Grass:
					attackType = grassProjectile;
					break;
				case ElementManager.Element.Wind:
					attackType = windProjectile;
					break;
			}

			if (attackType != null)
			{
				nextFireTime = Time.time + fireDelay;

				//Log warnings in case prefab slots were empty
				if (attackType.projectilePrefab == null)
					Debug.LogWarning($"Could not fire {attackType.Element} projectile: missing prefab");
				if (attackType.projectileCastEffectPrefab == null)
					Debug.LogWarning($"Could not do {attackType.Element} cast effect: missing prefab");

				Fire(attackType.projectilePrefab, attackType.projectileCastEffectPrefab, aimDirection);

				if (animator)
				{
					//Pass vertical axis into animator
					animator.SetFloat("vertical", aimDirection.y);
					//Set trigger for magic animation
					animator.SetTrigger("magic");
				}
			}
			else //If no attack was chosen, do fire fail anim
			{
				nextFireTime = Time.time + fireDelay;

				Fire(null, failedCastEffect, aimDirection);

				if (animator)
				{
					//Pass vertical axis into animator
					animator.SetFloat("vertical", aimDirection.y);
					//Set trigger for magic animation
					animator.SetTrigger("magic");
				}
			}
		}
	}

    void Fire(GameObject projectilePrefab, GameObject castEffect, Vector2 direction)
    {
        bool casted = false;

        //Projectile fire direction
        float angle = direction.magnitude > 0 ? Vector2.Angle(Vector2.right, direction) : 0;

        //Get fire point based on angle
        Transform firePoint = forwardFirePoint;

        if((int)angle == 45)
        {
            if (direction.y > 0)
                firePoint = upForwardFirePoint;
            else
                firePoint = downForwardFirePoint;
        }
        else if((int)angle == 90 || (int)angle == 135)
        {
            if (direction.y > 0)
                firePoint = upFirePoint;
            else
                firePoint = downFirePoint;
        }

        //Spawn projectile (if there is one)
        if (projectilePrefab)
        {
            //Get projectile from pool
            GameObject obj = ObjectPooler.GetPooledObject(projectilePrefab);

            casted = true;

            //Position projectile at fire point
            obj.transform.position = firePoint.position;

            //Get projectile script reference
            Projectile proj = obj.GetComponent<Projectile>();
            //Set as owner of projectile
            proj.SetOwner(gameObject);

            //Get projectile moving
            if (direction.magnitude > 0)
                proj.Fire(direction);
        }

        //If there is a cast effect
        if (castEffect)
        {
            //Get effect from pool
            GameObject effect = ObjectPooler.GetPooledObject(castEffect);

            //Set position and rotation
            effect.transform.position = firePoint.position;
            effect.transform.eulerAngles = new Vector3(0, 0, angle);

            //Make particles move how the player was (so they aren't left behind when the player is moving)
            ParticleSystem system = effect.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.VelocityOverLifetimeModule m = system.velocityOverLifetime;
            m.x = characterMove.Velocity.x;
            m.y = characterMove.Velocity.y;

            //Start the particle system (does not play on awake so values can be set through script beforehand)
            system.Play();
        }

		if(!casted)
			failedCastSound.Play(transform.position);
    }

    public void ResetMana()
    {
		Debug.LogWarning("Reset mana not implemented!");
    }

    IEnumerator BatCharge(float delay)
    {
        // Can only charge bat after landing
        while (!characterMove.IsGrounded)
            yield return null;

        isChargingBat = true;

        //Wait for max bat hold
        yield return new WaitForSeconds(delay);

        if (graphic)
        {
            Material mat = graphic.material;

            while (true)
            {
                //Flash off
                mat.SetFloat("_FlashAmount", 0);
                yield return new WaitForSeconds(flashInterval);

                //Flash on
                mat.SetFloat("_FlashAmount", flashAmount);
                yield return new WaitForSeconds(flashInterval);
            }
        }
    }
}
