using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnUpdateMagic;

    public bool canAttack = true;

	public enum MagicProgression
	{
		Basic,
		Half,
		Full
	}

	[Header("Magic")]
	public MagicProgression currentMagicProgression;
	[Space()]
	public ElementManager.Element baseMagicSelected;
	[Space()]
	public bool baseFireObtained;
	public bool baseGrassObtained;
	public bool baseIceObtained;
	public bool baseWindObtained;

	[System.Serializable]
	public class MixMagic
	{
		public ElementManager.Element element;
		public int currentMana = 100;
	}

	[Space()]
	public int maxMana = 100;
	public List<MixMagic> mixMagics = new List<MixMagic>();

	[Space()]
    public Transform upFirePoint;
    public Transform upForwardFirePoint;
    public Transform forwardFirePoint;
    public Transform downForwardFirePoint;
    public Transform downFirePoint;

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

    //Direction set by charactermove so that player doesnt have to hold x direction to fire
    private float directionX = 1;

    [Header("Absorb Magic")]
    public LayerMask pickupLayer;
    public float pickupRange = 2.0f;

    public float buttonHoldTime = 1.0f;

    [Space]
    public Transform magicAbsorbPoint;

	private MagicContainer container = null;

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
		if (OnUpdateMagic != null)
			OnUpdateMagic();

		//Should not have base magic selected unless there is no other magic unlocked
		if (baseMagicSelected == ElementManager.Element.None)
			SwitchBaseMagic();

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

    void Update()
    {
        //Get all colliders in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, pickupRange, pickupLayer);

        //Shortest distance starts at max
        float shortestDistance = float.MaxValue;

		bool found = false;

        //Loop through all pickup colliders (only pickups should be on this layer)
        foreach (Collider2D col in cols)
        {
			MagicContainer cont = col.GetComponent<MagicContainer>();

			if (cont)
			{
				//Calculate distance between player and pickup
				float distance = Vector3.Distance(col.transform.position, transform.position);

				//If this pickup is closer than any others...
				if (distance < shortestDistance)
				{
					//...make this pickup the new closest
					shortestDistance = distance;

					//Stop highlighting the old container
					if (container)
						container.Highlight(false);

					//Get the new container
					container = cont;

					//Highlight the new container
					if (container)
						container.Highlight(true);

					found = true;
				}
			}
        }

		//Deselect container if out of range
		if (!found && container)
		{
			container.Highlight(false);
			container = null;
		}
	}

    public void AbsorbMagic(bool buttonDown)
    {
		//Can't absorb while holding bat
		if (HoldingBat)
			return;

        //If the absorb button is pressed
        if (buttonDown)
        {
            //If there is a container, start absorbing it
            if (container)
                container.StartAbsorb(this);

            if (characterAnimator)
                characterAnimator.SetAbsorbing(true);
        }
        else
        {
            //If button was released, cancel absorption
            if (container)
                container.CancelAbsorb();

            container = null;

            if (characterAnimator)
                characterAnimator.SetAbsorbing(false);
		}
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

    //Function to use magic, wrapped by other magic use functions
    public void UseMagic()
    {
		if (!canAttack)
			return;

		//If magic slot was chosen correctly, and there is an attack in the slot
		if (Time.time >= nextFireTime)
		{
			MagicAttack attack = null;

			//Only try and mix magic if there is magic to mix
			if (mixMagics.Count > 0)
			{
				//If this magic has enough mana then mix, otherwise default (should only be empty if there are no mix magics left)
				if (mixMagics[0].currentMana > 0)
				{
					//Get mix magic
					attack = ElementManager.GetAttack(baseMagicSelected, mixMagics[0].element);

					if (attack)
					{
						//Remove mana cost
						mixMagics[0].currentMana -= attack.manaCost;

						//If out of mana, clear slot and cycle to next
						if (mixMagics[0].currentMana <= 0)
						{
							mixMagics[0].currentMana = 0;
							mixMagics[0].element = ElementManager.Element.None;

							SwitchMixMagic();
						}
					}
				}
				else
				{
					mixMagics[0].currentMana = 0;
					mixMagics[0].element = ElementManager.Element.None;

					SwitchMixMagic();

					attack = ElementManager.GetAttack(baseMagicSelected, ElementManager.Element.None);
				}
			}
			else
				attack = ElementManager.GetAttack(baseMagicSelected, ElementManager.Element.None);

			if (attack)
			{
				switch(attack.type)
				{
					case MagicAttack.Type.Projectile:
						nextFireTime = Time.time + fireDelay;

						Fire(attack, aimDirection);

						if (characterAnimator)
						{
							//Pass vertical axis into animator
							characterAnimator.animator.SetFloat("vertical", aimDirection.y);
							//Set trigger for magic animation
							characterAnimator.animator.SetTrigger("magic");
						}
						break;
					case MagicAttack.Type.Animation:
						nextFireTime = Time.time + attack.animation.length + fireDelay;

						if(characterAnimator)
						{
							characterAnimator.animator.Play(attack.animation.name);
						}
						break;
				}
			}
			else
			{
				nextFireTime = Time.time + fireDelay;

				Fire(null, aimDirection);

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

    void Fire(MagicAttack attack, Vector2 direction)
    {
        GameObject castEffect = failedCastEffect;
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
        if (attack && attack.projectilePrefab)
        {
            //Get projectile from pool
            GameObject obj = ObjectPooler.GetPooledObject(attack.projectilePrefab);

            castEffect = attack.castEffect;
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

	public void SwitchBaseMagic()
	{
		//Can only switch magic if they are unlocked
		if (baseFireObtained || baseGrassObtained || baseIceObtained || baseWindObtained)
		{
			int maxEnumInt = System.Enum.GetNames(typeof(ElementManager.Element)).Length - 1;

			//Get current magic as int and increment to next element
			int selectedMagic = (int)baseMagicSelected;
			selectedMagic++;

			//If magic is not obtained, move to next element
			if (selectedMagic == (int)ElementManager.Element.Fire && !baseFireObtained)
				selectedMagic++;
			if (selectedMagic == (int)ElementManager.Element.Grass && !baseGrassObtained)
				selectedMagic++;
			if (selectedMagic == (int)ElementManager.Element.Ice && !baseIceObtained)
				selectedMagic++;
			if (selectedMagic == (int)ElementManager.Element.Wind && !baseWindObtained)
				selectedMagic++;

			//Wrap back around to start
			if (selectedMagic > maxEnumInt)
				selectedMagic = 1; //1 is start since 0 is no element

			//Cast int back to element enum
			baseMagicSelected = (ElementManager.Element)selectedMagic;
		}
		else
			baseMagicSelected = ElementManager.Element.None;

		//Update magic UI to reflect changes
		UpdateMagic();
	}

	public void SwitchMixMagic()
	{
		if(mixMagics.Count > 1)
		{
			//Move first element to last in list
			MixMagic element = mixMagics[0];
			mixMagics.RemoveAt(0);
			mixMagics.Add(element);
		}

		///Move empty notches to back of list
		//Remove empty notches from list
		List<MixMagic> emptyNotches = new List<MixMagic>();
		for(int i = mixMagics.Count - 1; i >= 0; i--)
		{
			if(mixMagics[i].element == ElementManager.Element.None)
			{
				emptyNotches.Add(mixMagics[i]);
				mixMagics.RemoveAt(i);
			}
		}
		//Add empty notches back to end of list
		mixMagics.AddRange(emptyNotches);

		//Update magic UI to reflect changes
		UpdateMagic();
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

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, pickupRange);
	}
}
