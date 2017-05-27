using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnUpdateMagic;

    [System.Serializable]
    public class MagicSlot
    {
        public MagicAttack attack;
        public int currentMana;
        public float nextFireTime;
    }

    [Header("Magic")]
    public MagicSlot magicSlot1;
    public MagicSlot magicSlot2;

    [Space()]
    public Transform forwardFirePoint;
    public Transform upFirePoint;
    public Transform downFirePoint;

    [Space()]
    public float fireDelay = 0.1f;
    [Range(0, 1f)]
    [Tooltip("How far the vertical axis needs to be to fire vertical (like a dead-zone).")]
    public float upThreshold = 0.5f;

    [Space()]
    public GameObject failedCastEffect;

    //Direction set by charactermove so that player doesnt have to hold x direction to fire
    private float directionX = 1;

    [Header("Absorb Magic")]
    public LayerMask pickupLayer;
    public float pickupRange = 2.0f;

    public float buttonHoldTime = 1.0f;

    private MagicContainer container = null;

    [Header("Bat Swing")]
    public DamageOnEnable batSwing;
    public int damageAmount = 20;
    [Space()]
    [Tooltip("The minimum amount of time the bat needs to charge before a damage multiplier is applied.")]
    public float minHoldTime = 0.5f;
    [Tooltip("The time the bat needs to charge for the full damage multiplier to be applied.")]
    public float maxHoldTime = 2.0f;
    private float heldStartTime = 0;
    [Space()]
    public float heldDamageMultiplier = 1.5f;
    [Space()]
    public float moveSpeedMultiplier = 0.75f;
    private float oldMoveSpeed = 0f;

    [Space()]
    public SpriteRenderer graphic;
    public float flashAmount = 0.5f;
    public float flashInterval = 0.1f;

    //Struct to pass projectile information into coroutine
    private struct FireProjectile
    {
        public MagicAttack attack;
        public Vector2 direction;

        public FireProjectile(MagicAttack attack, Vector2 direction)
        {
            this.attack = attack;
            this.direction = direction;
        }
    }

    private PlayerActions playerActions;

    private CharacterAnimator characterAnimator;
    private CharacterMove characterMove;
    
    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        characterMove = GetComponent<CharacterMove>();
    }

    void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        //If there is an attack in the second slot, but not the first slot
        if (!magicSlot1.attack && magicSlot2.attack)
        {
            //Move second magic attack to first slot
            magicSlot1.attack = magicSlot2.attack;
            magicSlot2.attack = null;

            if (OnUpdateMagic != null)
                OnUpdateMagic();
        }

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
    }

    void Update()
    {
        //Get all colliders in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, pickupRange, pickupLayer);

        //Shortest distance starts at max
        float shortestDistance = float.MaxValue;

        //Deselect container if out of range
        if(cols.Length < 1 && container)
        {
            container.Highlight(false);
            container = null;
        }

        //Loop through all pickup colliders (only pickups should be on this layer)
        foreach (Collider2D col in cols)
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
                container = col.GetComponent<MagicContainer>();

                //Highlight the new container
                if(container)
                    container.Highlight(true);
            }
        }

        //If the absorb button is pressed
        if (playerActions.AbsorbMagic.WasPressed)
        {
            //If there is a container, start absorbing it
            if (container)
            {
                container.StartAbsorb(this);
            }
        }
        else if (playerActions.AbsorbMagic.WasReleased)
        {
            //If button was released, cancel absorption
            if (container)
                container.CancelAbsorb();

            container = null;
        }
    }

    public void UseMelee(bool holding)
    {
        //If button was released update bat swing damage
        if(batSwing && !holding)
        {
            batSwing.amount = damageAmount;

            //Calculate time held for lerp
            float time = Mathf.Clamp(Time.time - heldStartTime, minHoldTime, maxHoldTime);
            time -= minHoldTime;

            //Calculate damage multiplier between max and min hold times
            float multiplier = Mathf.Lerp(1.0f, heldDamageMultiplier, time / (maxHoldTime - minHoldTime));

            //Set multiplier
            batSwing.multiplier = multiplier;

            if (graphic)
            {
                //Stop flashing after bat swing
                StopCoroutine("BatFlash");
                graphic.material.SetFloat("_FlashAmount", 0);
            }
        }

        //Play melee animation
        if (characterAnimator && characterMove.canMove)
        {
            //Play charged attack anim if fully charged
            characterAnimator.SetCharged(Time.time >= heldStartTime + maxHoldTime && heldStartTime > 0);

            characterAnimator.MeleeAttack(holding);

            heldStartTime = 0;
        }

        //Keep track of time held for damage multiplier
        if (holding)
        {
            heldStartTime = Time.time;

            //Start bat flash after max hold time
            StartCoroutine("BatFlash", maxHoldTime);
        }

        //Reduce move speed while holding bat
        if (characterMove)
            characterMove.moveSpeed = holding ? oldMoveSpeed * moveSpeedMultiplier : oldMoveSpeed;
    }

    public void SwitchMagic()
    {
        MagicSlot temp = magicSlot1;
        magicSlot1 = magicSlot2;
        magicSlot2 = temp;

        UpdateMagic();
    }

    public void UpdateMagic()
    {
        if (OnUpdateMagic != null)
            OnUpdateMagic();
    }

    //Function to use magic, wrapped by other magic use functions
    public void UseMagic(Vector2 direction)
    {
        MagicSlot slot = magicSlot1;

        //If magic slot was chosen correctly, and there is an attack in the slot
        if (Time.time >= slot.nextFireTime)
        {
            if (slot != null && slot.attack)
            {
                slot.nextFireTime = Time.time + slot.attack.cooldownTime;

                //If there is enough mana to use this attack
                if (slot.currentMana >= slot.attack.manaCost)
                {
                    //Subtract required mana
                    slot.currentMana -= slot.attack.manaCost;

                    //Fire projectile after delay (timed to animation)
                    StartCoroutine("FireWithDelay", new FireProjectile(slot.attack, direction));
                }
                else
                {
                    StartCoroutine("FireWithDelay", new FireProjectile(null, direction));
                }
            }
            else
            {
                slot.nextFireTime = Time.time + 0.5f;

                StartCoroutine("FireWithDelay", new FireProjectile(null, direction));
            }

            if (characterAnimator)
            {
                //If y direction is within threshold, cancel it out
                if (direction.y < upThreshold && direction.y > -upThreshold)
                    direction.y = 0;

                //Pass vertical axis into animator
                characterAnimator.animator.SetFloat("vertical", direction.y);
                //Set trigger for magic animation
                characterAnimator.animator.SetTrigger("magic");
            }

            //If attack runs out of mana, it is lost
            if (slot.currentMana <= 0)
            {
                slot.attack = null;
                UpdateMagic();
            }
        }
    }

    IEnumerator FireWithDelay(FireProjectile fire)
    {
        yield return new WaitForSeconds(fireDelay);

        GameObject castEffect = failedCastEffect;

        //Default values for a projectile fired RIGHT
        Vector2 dir = Vector2.right;
        Transform firePoint = forwardFirePoint;
        float effectAngle = 0f;

        //Change values to suit projectiles fired in the other 3 directions
        if (fire.direction.y >= upThreshold)
        {
            dir = Vector2.up;
            firePoint = upFirePoint;
            effectAngle = 90f;
        }
        else if (fire.direction.y <= -upThreshold)
        {
            dir = Vector2.down;
            firePoint = downFirePoint;
            effectAngle = 270f;
        }
        else if (directionX < 0)
        {
            dir = Vector2.left;
            effectAngle = 180f;
        }

        //Spawn projectile (if there is one)
        if (fire.attack && fire.attack.projectilePrefab)
        {
            //Get projectile from pool
            GameObject obj = ObjectPooler.GetPooledObject(fire.attack.projectilePrefab);

            castEffect = fire.attack.castEffect;

            //Position projectile at fire point
            obj.transform.position = firePoint.position;

            //Get projectile script reference
            Projectile proj = obj.GetComponent<Projectile>();
            //Set as owner of projectile
            proj.SetOwner(gameObject);

            //Projectile should have the same element as the attack that fired it
            proj.element = fire.attack.element;

            //Get projectile moving
            proj.Fire(dir);
        }

        //If there is a cast effect
        if (castEffect)
        {
            //Get effect from pool
            GameObject effect = ObjectPooler.GetPooledObject(castEffect);

            //Set position and rotation
            effect.transform.position = firePoint.position;
            effect.transform.eulerAngles = new Vector3(0, 0, effectAngle);

            //Make particles move how the player was (so they aren't left behind when the player is moving)
            ParticleSystem system = effect.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.VelocityOverLifetimeModule m = system.velocityOverLifetime;
            m.x = characterMove.velocity.x;
            m.y = characterMove.velocity.y;

            //Start the particle system (does not play on awake so values can be set through script beforehand)
            system.Play();
        }
    }

    public void ResetMana()
    {
        if(magicSlot1 != null)
            magicSlot1.currentMana = magicSlot1.attack.manaAmount;
        if(magicSlot2 != null)
            magicSlot2.currentMana = magicSlot2.attack.manaAmount;
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
