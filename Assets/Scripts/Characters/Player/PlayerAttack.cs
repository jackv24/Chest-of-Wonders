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

	[System.Serializable]
	public class MagicAttackType
	{
		public GameObject projectilePrefab;
		public GameObject projectileCastEffectPrefab;

		public ElementManager.Element Element { get; private set; }

		public MagicAttackType(ElementManager.Element element)
		{
			Element = element;
		}
	}

	public MagicAttackType fireMagic = new MagicAttackType(ElementManager.Element.Fire);
	public MagicAttackType grassMagic = new MagicAttackType(ElementManager.Element.Grass);
	public MagicAttackType iceMagic = new MagicAttackType(ElementManager.Element.Ice);
	public MagicAttackType windMagic = new MagicAttackType(ElementManager.Element.Wind);

	public ElementManager.Element selectedElement = ElementManager.Element.None;

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
    public SoundEffectBase.SoundEffect failedCastSound;

    [Space()]
    public float arrowDistance = 1.0f;
    public float arrowHeight = 0.5f;
    public Transform arrow;

    private Vector2 aimDirection;
    private int lastDiagonalDirection = 1;

    //Direction set by character move so that player doesn't have to hold x direction to fire
    private float directionX = 1;

    [Header("Bat Swing")]
    public DamageOnEnable batSwing;
    public DamageOnEnable chargedBatSwing;
    public int damageAmount = 20;
    [Space()]
    [Tooltip("The minimum amount of time the bat needs to charge before a damage multiplier is applied.")]
    public float chargeHoldTime = 1.0f;
    private float heldStartTime = 0;
    public float heldDamageMultiplier = 1.5f;

	private bool holdingBat = false;
	public bool HoldingBat { get { return holdingBat; } }

    [Space()]
    public float moveSpeedMultiplier = 0.75f;
    private float oldMoveSpeed = 0f;

    [Space()]
    public SpriteRenderer graphic;
    public float flashAmount = 0.5f;
    public float flashInterval = 0.1f;

    private CharacterAnimator characterAnimator;
    private CharacterMove characterMove;
    private CharacterSound characterSound;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        characterMove = GetComponent<CharacterMove>();
        characterSound = GetComponent<CharacterSound>();
    }

    void Start()
    {
		//Update magic UI to start
		UpdateMagic();

		//Create event handler to update the players facing direction
		if (characterMove)
        {
            characterMove.OnChangedDirection += delegate (float newDir) { directionX = newDir; };

            oldMoveSpeed = characterMove.moveSpeed;
        }
    }

    void OnEnable()
    {
        if (characterMove && oldMoveSpeed != 0)
            characterMove.moveSpeed = oldMoveSpeed;

        canAttack = true;
    }

    public void UseMelee(bool holding, float verticalDirection)
    {
        if (!canAttack)
            return;

        DamageOnEnable swing = batSwing;

		holdingBat = holding;

        //If button was released update bat swing damage
        if(swing && !holding)
        {
            //Used charged bat swing collider if held for max hold time
            if (Time.time >= heldStartTime + chargeHoldTime)
                swing = chargedBatSwing;

            swing.amount = damageAmount;

            if (graphic)
            {
                //Stop flashing after bat swing
                StopCoroutine("BatFlash");
                graphic.material.SetFloat("_FlashAmount", 0);
            }
        }

        //Play melee animation
        if (characterAnimator)
        {
            //Play charged attack anim if fully charged
            characterAnimator.SetCharged(Time.time >= heldStartTime + chargeHoldTime && heldStartTime > 0);

            characterAnimator.MeleeAttack(holding, verticalDirection);

            heldStartTime = 0;
        }

        //Keep track of time held for damage multiplier
        if (holding)
        {
            heldStartTime = Time.time;

            //Start bat flash after max hold time
            StartCoroutine("BatFlash", chargeHoldTime);
        }

        //Reduce move speed while holding bat
        if (characterMove)
            characterMove.moveSpeed = holding ? oldMoveSpeed * moveSpeedMultiplier : oldMoveSpeed;
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

    //Function to use magic, wrapped by other magic use functions
    public void UseMagic()
    {
		if (!canAttack || magicProgression <= MagicProgression.Basic)
			return;

		//If magic slot was chosen correctly, and there is an attack in the slot
		if (Time.time >= nextFireTime)
		{
			MagicAttackType attackType = null;

			switch(selectedElement)
			{
				case ElementManager.Element.Fire:
					attackType = fireMagic;
					break;
				case ElementManager.Element.Ice:
					attackType = iceMagic;
					break;
				case ElementManager.Element.Grass:
					attackType = grassMagic;
					break;
				case ElementManager.Element.Wind:
					attackType = windMagic;
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

				if (characterAnimator)
				{
					//Pass vertical axis into animator
					characterAnimator.animator.SetFloat("vertical", aimDirection.y);
					//Set trigger for magic animation
					characterAnimator.animator.SetTrigger("magic");
				}
			}
			else //If no attack was chosen, do fire fail anim
			{
				nextFireTime = Time.time + fireDelay;

				Fire(null, failedCastEffect, aimDirection);

				if (characterAnimator)
				{
					//Pass vertical axis into animator
					characterAnimator.animator.SetFloat("vertical", aimDirection.y);
					//Set trigger for magic animation
					characterAnimator.animator.SetTrigger("magic");
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
            m.x = characterMove.velocity.x;
            m.y = characterMove.velocity.y;

            //Start the particle system (does not play on awake so values can be set through script beforehand)
            system.Play();
        }

        if(!casted && characterSound)
        {
            characterSound.PlaySound(failedCastSound);
        }
    }

    public void ResetMana()
    {
		Debug.LogWarning("Reset mana not implemented!");
    }

    IEnumerator BatFlash(float delay)
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
}
