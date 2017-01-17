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

    [Tooltip("The rate at which the character accelerates to reach the move speed (m/s^2).")]
    public float acceleration = 1f;

    [Space()]
    [Tooltip("The maximum angle at which a slope is considered walkable.")]
    public float slopeLimit = 50f;

    [HideInInspector]
    public float inputDirection = 0f;

    [HideInInspector]
    public bool canMove = true;

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

    [Header("Misc")]
    public float knockBackRecoveryTime = 0.25f;

    //The RigidBody2D attached to this GameObject
    [HideInInspector]
    public Rigidbody2D body;
    private CharacterAnimator characterAnimator;

    private Collider2D col;
    private Rect box;

    [HideInInspector]
    public Vector2 velocity;

    private void Awake()
    {
        //Get references
        col = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Start()
    {
        //Movemement is not handled by rigidbody until knocked back
        if(body)
            body.simulated = false;
    }

    private void Update()
    {
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
                hits[i] = Physics2D.Raycast(origin, (velocity.y > 0 ? Vector2.up : Vector2.down), distance, groundLayer);

                Debug.DrawLine(origin, new Vector2(origin.x, origin.y + Mathf.Sign(velocity.y) * distance));

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

            //If a ray has connected witht the ground
            if (connected)
            {
                //Set grounded and stop falling (if already falling)
                if (velocity.y <= 0)
                    isGrounded = true;
                else //If moving upwards, stop jumping
                    heldJump = false;
                velocity.y = 0;

                //Move player flush to ground (using shortest ray)
                transform.Translate(Vector2.down * (hits[index].distance - box.height / 2));
            }
        }

        //Horizontal movement
        if(canMove)
            velocity.x = Mathf.Lerp(velocity.x, moveSpeed * inputDirection, acceleration * Time.deltaTime);

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
                        if (angle > slopeLimit)
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

        //Move character by velocity
        transform.Translate(velocity * Time.deltaTime);
    }


    public void Move(float direction)
    {
        //Cache old direction for comparison
        oldDirection = inputDirection;

        //Update input direction
        if (GameManager.instance.gameRunning && canMove)
            inputDirection = direction;
        else
            inputDirection = 0;

        //If direction has changed (and does not equal 0), then call changed direction event
        if (inputDirection != oldDirection && direction != 0 && OnChangedDirection != null)
            OnChangedDirection(direction);
    }

    public void Jump(bool pressed)
    {
        if (GameManager.instance.gameRunning && canMove)
        {
            if (pressed)
                shouldJump = true;

            heldJump = pressed;
        }
    }

    public void Knockback(Vector2 centre, float amount)
    {
        //If this gameobject is active
        if (gameObject.activeInHierarchy)
        {
            //Start the knockback delay coruotine
            StartCoroutine("SwitchToPhysics", knockBackRecoveryTime);

            //Calculate force direction
            Vector2 direction = (Vector2)transform.position - centre;
            direction.Normalize();

            //Apply force
            body.AddForceAtPosition(direction * amount, centre, ForceMode2D.Impulse);
        }
    }

    IEnumerator SwitchToPhysics(float timeAfterGrounded)
    {
        if (!body)
            Debug.LogWarning("There is no Rigidbody2D attached to " + name);
        else
        {
            //Disable movement
            canMove = false;
            body.simulated = true;

            //Continue until grounded
            while (!isGrounded)
            {
                yield return new WaitForEndOfFrame();
            }

            //After grounded, wait for specified time
            yield return new WaitForSeconds(timeAfterGrounded);

            //Re-enable movement
            canMove = true;
            body.simulated = false;
        }
    }
}
