using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnSwitchMagic;

    [System.Serializable]
    public class MagicSlot
    {
        public MagicAttack attack;
        public int currentMana;
        public float nextFireTime;
    }

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

    private CharacterAnimator characterAnimator;
    private CharacterMove characterMove;
    
    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        characterMove = GetComponent<CharacterMove>();
    }

    void Start()
    {
        //If there is an attack in the second slot, but not the first slot
        if (!magicSlot1.attack && magicSlot2.attack)
        {
            //Move second magic attack to first slot
            magicSlot1.attack = magicSlot2.attack;
            magicSlot2.attack = null;
        }

        //Set starting mana
        if (magicSlot1.attack)
            magicSlot1.currentMana = magicSlot1.attack.manaAmount;
        if (magicSlot2.attack)
            magicSlot2.currentMana = magicSlot2.attack.manaAmount;

        //Create event handler to update the players facing direction
        if (characterMove)
            characterMove.OnChangedDirection += delegate (float newDir) { directionX = newDir; };
    }

    public void UseMelee()
    {
        //Play melee animation
        if (characterAnimator && characterMove.canMove)
            characterAnimator.MeleeAttack();
    }

    public void SwitchMagic()
    {
        MagicSlot temp = magicSlot1;
        magicSlot1 = magicSlot2;
        magicSlot2 = temp;

        if (OnSwitchMagic != null)
            OnSwitchMagic();
    }

    //Function to use magic, wrapped by other magic use functions
    public void UseMagic(Vector2 direction)
    {
        MagicSlot slot = magicSlot1;

        //If magic slot was chosen correctly, and there is an attack in the slot
        if(slot != null && slot.attack && Time.time >= slot.nextFireTime)
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
                StartCoroutine("FireWithDelay", new FireProjectile(null, direction));

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
}
