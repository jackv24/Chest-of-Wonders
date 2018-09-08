using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	public delegate void UpdateMagicEvent();
    public event UpdateMagicEvent OnUpdateMagic;
	public event UpdateMagicEvent OnSwitchMagic;

	public enum MagicProgression
	{
		Basic,
		Full
	}
	public MagicProgression magicProgression;

	public bool canAttack = true;

	[Header("Magic")]
	public bool hasFireMagic;
	public bool hasGrassMagic;
	public bool hasIceMagic;
	public bool hasWindMagic;

	public ElementManager.Element selectedElement = ElementManager.Element.None;

	#region Magic Melee Attack Animations

	//Will build animation state name strings at runtime e.g, "Fire Special Ground D"
	private const string magicMeleeAnimTemplate = "{0} Special {1} {2}";

	private const string magicMeleeElementFire = "Fire";
	private const string magicMeleeElementIce = "Ice";
	private const string magicMeleeElementGrass = "Grass";
	private const string magicMeleeElementWind = "Wind";

	private const string magicMeleeStateGround = "Ground";
	private const string magicMeleeStateAir = "Air";

	private const string magicMeleeDirectionSide = "LR";
	private const string magicMeleeDirectionUp = "U";
	private const string magicMeleeDirectionDown = "D";

	#endregion

	[System.Serializable]
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

	[Space()]
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

	public bool HoldingBat { get; private set; } = false;

	private Coroutine batChargeRoutine = null;

    [Space()]
    public SpriteRenderer graphic;
    public float flashAmount = 0.5f;
    public float flashInterval = 0.1f;

    private CharacterMove characterMove;
	private Animator animator;
	private CharacterAnimator characterAnimator;

    private void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
		characterAnimator = GetComponent<CharacterAnimator>();

		animator = graphic.GetComponent<Animator>();
    }

    void Start()
    {
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				magicProgression = data.MagicProgression;

				selectedElement = data.SelectedElement;

				hasFireMagic = data.HasFireMagic;
				hasGrassMagic = data.HasGrassMagic;
				hasIceMagic = data.HasIceMagic;
				hasWindMagic = data.HasWindMagic;

				UpdateMagic();
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				data.MagicProgression = magicProgression;

				data.SelectedElement = selectedElement;

				data.HasFireMagic = hasFireMagic;
				data.HasGrassMagic = hasGrassMagic;
				data.HasIceMagic = hasIceMagic;
				data.HasWindMagic = hasWindMagic;
			};
		}

		//Update magic UI to start
		UpdateMagic();

		//Create event handler to update the players facing direction
		if (characterMove)
        {
            characterMove.OnChangedDirection += delegate (float newDir) { directionX = newDir; };
        }
    }

    void OnEnable()
    {
        canAttack = true;
    }

    public void UseMelee(bool buttonDown, float verticalDirection)
    {
		StopBatCharge();

		if (canAttack)
		{
			//Button pressed
			if(buttonDown && !HoldingBat)
			{
				heldStartTime = Time.time;

				batChargeRoutine = StartCoroutine(BatCharge(chargeHoldTime));

				animator?.SetTrigger("batSwing");
			}
			else if(!buttonDown) //Button released
			{
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
		HoldingBat = value;

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

    public void UpdateMagic()
    {
        if (OnUpdateMagic != null)
            OnUpdateMagic();
    }

	public void SwitchMagic(int direction)
	{
		if (magicProgression <= MagicProgression.Basic)
			return;

		//Make sure direction is not larger than 1
		direction = (int)Mathf.Sign(direction);

		int selected = (int)selectedElement;
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
					if (hasFireMagic)
						success = true;
					break;
				case ElementManager.Element.Grass:
					if (hasGrassMagic)
						success = true;
					break;
				case ElementManager.Element.Ice:
					if (hasIceMagic)
						success = true;
					break;
				case ElementManager.Element.Wind:
					if (hasWindMagic)
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
		{
			selectedElement = (ElementManager.Element)selected;

			if (OnSwitchMagic != null)
				OnSwitchMagic();

			UpdateMagic();
		}
	}

    public void UseProjectileMagic()
    {
		if (!canAttack || magicProgression <= MagicProgression.Basic)
			return;

		//If magic slot was chosen correctly, and there is an attack in the slot
		if (Time.time >= nextFireTime)
		{
			ProjectileAttack attackType = null;

			switch(selectedElement)
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

	public void UseMagicMeleeAttack(Vector2 inputDirection)
	{
		if (!canAttack || selectedElement == ElementManager.Element.None)
			return;

		//Snap input vector to 4 directions
		Vector2 direction = Helper.SnapTo(inputDirection, 90.0f).normalized;

		string animName = GetMagicMeleeStateName(selectedElement, characterMove.IsGrounded, direction.y);

		//Play attack animation, and prevent further attacks while in this animation (animation state itself will handle attack with StateMachineBehaviours)
		canAttack = false;
		characterAnimator.Play(animName, () =>
		{
			canAttack = true;

			characterAnimator.ReturnToLocomotion();
		});
	}

	/// <summary>
	/// Builds a state name string using the given parameters to be used for playing a magic melee animation.
	/// </summary>
	private string GetMagicMeleeStateName(ElementManager.Element element, bool isGrounded, float verticalDirection)
	{
		string meleeElementName;
		switch(element)
		{
			case ElementManager.Element.Fire:
				meleeElementName = magicMeleeElementFire;
				break;
			case ElementManager.Element.Grass:
				meleeElementName = magicMeleeElementGrass;
				break;
			case ElementManager.Element.Ice:
				meleeElementName = magicMeleeElementIce;
				break;
			case ElementManager.Element.Wind:
				meleeElementName = magicMeleeElementWind;
				break;
			default:
				meleeElementName = "NONE";
				break;
		}

		string directionName;
		if (verticalDirection > 0)
			directionName = magicMeleeDirectionUp;
		else if (verticalDirection < 0)
			directionName = magicMeleeDirectionDown;
		else
			directionName = magicMeleeDirectionSide;

		return string.Format(magicMeleeAnimTemplate, meleeElementName, isGrounded ? magicMeleeStateGround : magicMeleeStateAir, directionName);
	}
}
