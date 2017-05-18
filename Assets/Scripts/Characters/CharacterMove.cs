using UnityEngine;
using System.Collections;

public class CharacterMove : MonoBehaviour
{
    public delegate void ChangeFloat(float newFloat);
    public delegate void GeneralEvent();

    public event ChangeFloat OnChangedDirection;
    private float oldDirection;
    public event GeneralEvent OnJump;

    [Header("Movement")]
    [Tooltip("The horizontal move speed (m/s).")]
    public float moveSpeed = 2f;
    //What fraction of the move speed to actually move at (move speed is dampened on slopes)
    private float slopeSpeedMultiplier = 1;

    [Tooltip("The rate at which the character accelerates to reach the move speed (m/s^2).")]
    public float acceleration = 1f;

    [Space()]
    [Tooltip("The maximum angle at which a slope is considered walkable upwards.")]
    public float upSlopeLimit = 50f;
    [Tooltip("The maximum angle at which a slope is considered walkable downwards (otherwise falling).")]
    public float downSlopeLimit = 30f;
    [Tooltip("The maximum distance at which a slope will be considered (prevents player snapping to slopes from great heights).")]
    public float maxSlopeDistance = 1f;
    [Tooltip("Should horizontal speed be dampened on slopes?")]
    public bool slopeSpeedDampening = true;

    [HideInInspector]
    public float inputDirection = 0f;

    //[HideInInspector]
    public bool canMove = true;
    public bool ignoreCanMove = false;

    [Header("Jumping")]
    [Tooltip("Indirectly controls jump height (this force is applied every fram jump is held, but in smaller and smaller fractions.")]
    public float jumpForce = 10f;

    [Tooltip("How long the jump button can be held for before the character starts falling.")]
    public float jumpTime = 1f;
    private float jumpHeldTime = 0;

    [Space()]
    [Tooltip("How long after the character leaves the ground until they can no longer jump (recommended to have this delay for platformers).")]
    public float stopJumpDelay = 0.02f;
    private float stopJumpTime;

    private bool shouldJump = false;
    private bool heldJump = false;

    [HideInInspector]
    public bool isGrounded = false;
    private bool wasGrounded = false;

    private bool stickToSlope = false;

    [Header("Physics")]
    [Tooltip("How fast the character falls (m/s^2).")]
    public float gravity = 10f;
    [Tooltip("The maximum speed at which the character can fall (otherwise known as terminal velocity).")]
    public float maxFallSpeed = 20f;

    [Space()]
    [Tooltip("How many rays to cast down and up for collision detection.")]
    public int verticalRays = 3;
    [Tooltip("How many rays to cast left and right for collision detection.")]
    public int horizontalRays = 5;
    [Tooltip("How far inside the characters collider should the rays start from? (should be greater than 0)")]
    public float skinWidth = 0.01f;

    [Space()]
    public LayerMask groundLayer;

    [Header("Miscellaneous")]
    public float knockBackRecoveryTime = 1f;
    public bool allowKnockback = true;
    [HideInInspector]
    public bool scriptControl = true;

    //The RigidBody2D attached to this GameObject
    [HideInInspector]
    public Rigidbody2D body;

    private Collider2D col;
    private Rect box;

    [HideInInspector]
    public Vector2 velocity;

    private CharacterAnimator characterAnimator;
    private CharacterStats characterStats;
    private CharacterSound characterSound;

    private void Awake()
    {
        //Get references
        col = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();

        characterAnimator = GetComponent<CharacterAnimator>();
        characterStats = GetComponent<CharacterStats>();
        characterSound = GetComponent<CharacterSound>();
    }

    void Start()
    {
        if(characterSound)
        {
            OnJump += delegate { characterSound.PlaySound(characterSound.jumpSound); };
        }
    }

    private void OnEnable()
    {
        canMove = true;

        if (body)
            body.isKinematic = true;
    }

    private void Update()
    {
        //Only run if script should control movement
        if (!scriptControl)
            return;

        //Store collider rect for easy typing
        box = new Rect(
            col.bounds.min.x,
            col.bounds.min.y,
            col.bounds.size.x,
            col.bounds.size.y
            );

        //Apply gravity, capping fall speed
        velocity.y = Mathf.Max(velocity.y - gravity * Time.deltaTime, -maxFallSpeed);

        //Jumping
        {
            if (isGrounded)
                stopJumpTime = Time.time + stopJumpDelay;

            //If jump button has been pressed
            if (shouldJump)
            {
                //Consume jump button flag
                shouldJump = false;

                //If jump button was pressed within a small amount of time after leaving the ground (or still on ground)
                if (Time.time <= stopJumpTime)
                {
                    //Call jump events
                    if (OnJump != null)
                        OnJump();

                    //Reset jump held time, allowing jump to be held
                    jumpHeldTime = 0;
                    stopJumpTime = 0;

                    //Should not stick to slope when jumping
                    stickToSlope = false;
                    //Slope speed multiplier should be reset, since character is no longer on slope
                    slopeSpeedMultiplier = 1f;

                    wasGrounded = false;
                }
            }

            //If jump has been held for less than the max time
            if (heldJump && jumpHeldTime < jumpTime)
            {
                //Set velocity, slowly decreasing to zero over time
                velocity.y = Mathf.Lerp(jumpForce, 0, jumpHeldTime / jumpTime);

                //Count time jump is held
                jumpHeldTime += Time.deltaTime;
            }
            else //If jump has been released, at can not be held again until a new jump is started
                jumpHeldTime = jumpTime;
        }

        //Vertical collision detection
        {
            //Calculate start and end points that rays will be cast from between
            Vector2 startPoint = new Vector2(box.xMin + skinWidth, box.center.y);
            Vector2 endPoint = new Vector2(box.xMax - skinWidth, box.center.y);

            //Distance of rays should (when cast from centre) extend past the bottom by velocity amount (or skin width if grounded)
            float distance = box.height / 2 + (isGrounded ? skinWidth : Mathf.Abs(velocity.y * Time.deltaTime));

            //Not grounded unless a ray connects
            isGrounded = false;

            //All raycasts
            RaycastHit2D[] hits = new RaycastHit2D[verticalRays];
            bool connected = false;

            //Keep track of the shortest ray
            float minDistance = Mathf.Infinity;
            int index = 0;

            //Loops through and cast rays
            for(int i = 0; i < verticalRays; i++)
            {
                //Get origin between start and end points
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, i / (float)(verticalRays - 1));

                //Cast ray
                if (velocity.y > 0)
                    hits[i] = Physics2D.Raycast(origin, Vector2.up, distance, groundLayer);
                else
                    hits[i] = Physics2D.Raycast(origin, Vector2.down, maxSlopeDistance, groundLayer);

                Debug.DrawLine(origin, new Vector2(origin.x, origin.y + Mathf.Sign(velocity.y) * (velocity.y > 0 ? distance : maxSlopeDistance)));

                //If ray connected then player should be considered grounded
                if (hits[i].collider != null)
                {
                    connected = true;

                    //If this ray is shorter than any other, set it as the shortest and one to use
                    if (hits[i].distance < minDistance)
                    {
                        minDistance = hits[i].distance;
                        index = i;
                    }

                }
            }

            //If a ray has connected with the ground
            if (connected)
            {
                float angle = Vector2.Angle(Vector2.up, hits[index].normal);

                //If within distance, or if it should stick to the slope
                if (hits[index].distance <= distance || (angle <= downSlopeLimit && stickToSlope))
                {
                    //Set grounded and stop falling (if already falling)
                    if (velocity.y <= 0)
                    {
                        isGrounded = true;
                        stickToSlope = true;

                        //Calculate speed dampening based on slope (if slope dampening is not desired, make a value of 1)
                        slopeSpeedMultiplier = slopeSpeedDampening ? Mathf.Cos(angle * Mathf.Deg2Rad) : 1;
                    }
                    else
                        //If moving upwards, stop jumping
                        heldJump = false;

                    velocity.y = 0;

                    //Move player flush to ground (using shortest ray)
                    transform.Translate(Vector2.down * (hits[index].distance - box.height / 2));
                }
                else
                    //Prevent character from snapping to ground when the slope was not within the limit
                    stickToSlope = false;
            }
        }

        //Horizontal movement
        if ((canMove || ignoreCanMove))
        {
            velocity.x = Mathf.Lerp(velocity.x, moveSpeed * slopeSpeedMultiplier * inputDirection, acceleration * Time.deltaTime);
        }

        //Lateral collision detection
        {
            //Only required if there is lateral movement
            if (velocity.x != 0)
            {
                //Start and end points that ray origins will lay between
                Vector2 startPoint = new Vector2(box.center.x, box.yMin + skinWidth);
                Vector2 endPoint = new Vector2(box.center.x, box.yMax - skinWidth);

                //Rays are cast out according to velocity, from the center
                float distance = box.width / 2 + Mathf.Abs(velocity.x * Time.deltaTime);

                //Rays are cast in the direction of movement
                Vector2 direction = velocity.x > 0 ? Vector2.right : Vector2.left;

                //Cast required amount of rays
                for (int i = 0; i < horizontalRays; i++)
                {
                    //Calculate origin for this ray
                    Vector2 origin = Vector2.Lerp(startPoint, endPoint, i / (float)(horizontalRays - 1));

                    //Cast ray
                    RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);

                    Debug.DrawLine(origin, new Vector2(origin.x + distance * direction.x, origin.y));

                    //If ray hit, there is a wall
                    if (hit.collider != null)
                    {
                        //Calculate angle of slope
                        float angle = Vector2.Angle(Vector2.up, hit.normal);

                        //This is only considered a wall if outside slope limit
                        if (angle > upSlopeLimit)
                        {
                            //Make character flush against wall
                            transform.Translate(direction * (hit.distance - box.width / 2));

                            //Cease any lateral movement
                            velocity.x = 0;
                        }

                        //If one ray has connected, no more rays should be cast
                        break;
                    }
                }
            }
        }

        if (characterSound)
        {
            //Play sound when character lands on ground
            if (!wasGrounded && isGrounded)
            {
                wasGrounded = true;

                characterSound.PlaySound(characterSound.landSound);
            }
        }

        //Move character by velocity
        transform.Translate(velocity * Time.deltaTime);
    }

    public void Move(float direction)
    {
        //Cache old direction for comparison
        oldDirection = inputDirection;

        //Update input direction
        if ((GameManager.instance.CanDoActions || ignoreCanMove) && (canMove || ignoreCanMove))
            inputDirection = direction < 0 ? Mathf.Floor(direction) : Mathf.Ceil(direction);
        else
            inputDirection = 0;

        //If direction has changed (and does not equal 0), then call changed direction event
        if (inputDirection != oldDirection && direction != 0 && OnChangedDirection != null)
            OnChangedDirection(direction);
    }

    public void Jump(bool pressed)
    {
        if (GameManager.instance.CanDoActions && canMove)
        {
            if (pressed)
                shouldJump = true;

            heldJump = pressed;
        }
    }

    public void Knockback(Vector2 origin, float magnitude)
    {
        if (!allowKnockback)
            return;

        if (!gameObject.activeSelf)
            return;

        //Calculate force
        Vector2 direction = ((Vector2)transform.position - origin).normalized;
        Vector2 force = direction * magnitude;

        //Disable script movement
        scriptControl = false;
        //Enable rigidbody movement
        body.bodyType = RigidbodyType2D.Dynamic;

        //Apply force
        body.AddForceAtPosition(force, origin, ForceMode2D.Impulse);

        //Coroutine to switch back to script control when required
        StopCoroutine("KnockbackRecovery"); //Make sure only one is running
        StartCoroutine("KnockbackRecovery");
    }

    IEnumerator KnockbackRecovery()
    {
        if (characterAnimator)
            characterAnimator.SetStunned(true);

        //If body is still moving, cannot recover
        while (body.velocity.magnitude > 0.01f)
        {
            yield return new WaitForEndOfFrame();
        }

        //After body has stopped moving, wait alotted recover time
        yield return new WaitForSeconds(knockBackRecoveryTime);

        //Switch back to script control
        body.bodyType = RigidbodyType2D.Kinematic;
        scriptControl = true;

        if (characterStats)
        {
            //If character is not dead...
            if (!characterStats.IsDead)
            {
                //Re-enable movement
                canMove = true;

                //Set not stunned
                if (characterAnimator)
                    characterAnimator.SetStunned(false);
            }
        }
        else //If there is no stats just re-enable everything anyway
        {
            canMove = true;

            if (characterAnimator)
                characterAnimator.SetStunned(false);
        }

    }
}
