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

    [Tooltip("The rate at which the character accelerates to reach the move speed.")]
    public float acceleration = 1f;

    [HideInInspector]
    public float inputDirection = 0f;

    [HideInInspector]
    public bool canMove = true;

    [Header("Jumping")]
    public float jumpForce = 10f;

    public float jumpTime = 1f;
    private float jumpHeldTime = 0;

    [Space()]
    [Tooltip("How long after the player leaves the ground until they can no longer jump (recommended to have this delay for platformers).")]
    public float stopJumpDelay = 0.02f;
    private float stopJumpTime;

    private bool shouldJump = false;

    [HideInInspector]
    public bool isGrounded = false;

    [Header("Physics")]
    public float gravity = 10f;
    public float maxFallSpeed = 20f;

    [Space()]
    public int verticalRays = 3;
    public int horizontalRays = 5;
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
    private Vector2 velocity;

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

        //Vertical collision detection (scoped for variable naming convenience)
        {
            //Calculate start and end points that rays will be cast from between
            Vector2 startPoint = new Vector2(box.xMin + skinWidth, box.center.y);
            Vector2 endPoint = new Vector2(box.xMax - skinWidth, box.center.y);

            //Distance of rays should (when cast from centre) extend past the bottom by velocity amount (or skin width if grounded)
            float distance = box.height / 2 + (isGrounded ? skinWidth : Mathf.Abs(velocity.y * Time.deltaTime));

            //Not grounded unless a ray connects
            isGrounded = false;

            //Loops through and cast rays
            for(int i = 0; i < verticalRays; i++)
            {
                //Get origin between start and end points
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, i / (float)(verticalRays - 1));

                //Cast ray
                RaycastHit2D hit = Physics2D.Raycast(origin, (velocity.y > 0 ? Vector2.up : Vector2.down), distance, groundLayer);

                Debug.DrawLine(origin, new Vector2(origin.x, origin.y + Mathf.Sign(velocity.y) * distance));

                //If ray connected then player should be considered grounded
                if (hit.collider != null)
                {
                    //Set grounded and stop falling
                    isGrounded = true;
                    velocity.y = 0;

                    //Move player flush to ground
                    transform.Translate(Vector2.down * (hit.distance - box.height / 2));
                    
                    //If one ray has connected then it is grounded
                    break;
                }
            }
        }

        //Horizontal movement
        velocity.x = Mathf.Lerp(velocity.x, moveSpeed * inputDirection, acceleration * Time.deltaTime);

        //Lateral collision detection
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
                    //Make character flush against wall
                    transform.Translate(direction * (hit.distance - box.width / 2));

                    //Cease any lateral movement
                    velocity.x = 0;

                    //If one ray has connected, no more rays should be cast
                    break;
                }
            }
        }

        //Jumping
        if (isGrounded)
            stopJumpTime = Time.time + stopJumpDelay;

        //If jump button has been pressed
        if (shouldJump)
        {
            //Jump should be performed once
            shouldJump = false;

            if (Time.time <= stopJumpTime)
            {
                //Set upwards jump velocity
                velocity.y = jumpForce;

                //Call jump events
                if (OnJump != null)
                    OnJump();
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
        inputDirection = direction;

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
