using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMove : MonoBehaviour
{
    public delegate void ChangeFloat(float newFloat);
    public event ChangeFloat OnChangedDirection;
    private float oldDirection;

    [Header("Movement")]
    [Tooltip("The horizontal move speed (m/s).")]
    public float moveSpeed = 2f;

    [Tooltip("The rate at which the character accelerates to reach the move speed.")]
    public float acceleration = 1f;

    private float inputDirection = 0f;
    public float InputDirection { get { return inputDirection; } }

    public bool canMove = true;

    [Header("Jumping")]
    public float jumpForce = 10f;

    public float jumpTime = 1f;
    private float jumpHeldTime = 0;

    [Space()]
    [Tooltip("How long after the player leaves the ground until they can no longer jump (recommended to have this delay for platformers).")]
    public float stopJumpDelay = 0.02f;
    private float stopJumpTime;

    private bool pressedJump = false;
    private bool canJump = false;
    private bool heldJump = false;

    private bool isGrounded = true;
    public bool IsGrounded { get { return isGrounded; } }
    [Space()]
    public LayerMask groundLayer;
    [Space()]
    public Vector2 circleCastOrigin;
    public float circleCastRadius = 0.5f;
    public float groundedDistance = 0.01f;

    //The vector to add to the velocity
    private Vector2 moveVector = Vector2.zero;

    //The RigidBody2D attached to this GameObject
    private Rigidbody2D body;

    private void Awake()
    {
        //Get references
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        if (isGrounded)
            stopJumpTime = Time.time + stopJumpDelay;

        //Get current velocity
        moveVector = body.velocity;

        //Accelerate to reach target move speed
        moveVector.x = Mathf.Lerp(moveVector.x, inputDirection * moveSpeed, acceleration * Time.fixedDeltaTime);

        //Allow jump after button press
        if (pressedJump)
        {
            pressedJump = false;

            //Only jump if grounded, or just walked off a ledge
            if (isGrounded || Time.time < stopJumpTime)
            {
                //Reset jump time and allow jumping
                stopJumpTime = 0;
                canJump = true;
            }
        }

        //Jump as long as button is held (up to a point)
        if (canJump && heldJump && jumpHeldTime <= jumpTime)
        {
            //Keep track of time jumping
            jumpHeldTime += Time.fixedDeltaTime;

            //Lerp velocity down over jump (for the natural look)
            moveVector.y = Mathf.Lerp(jumpForce, 0, jumpHeldTime / jumpTime);
        }
        else
            canJump = false;

        //Stop from sticking to ceilings when jump is held
        if (canJump && jumpHeldTime > 0.05f && body.velocity.y <= 0)
            canJump = false;

        //Set velocity at end
        body.velocity = moveVector;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + circleCastOrigin, circleCastRadius);
    }

    private bool CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + circleCastOrigin;

        RaycastHit2D hit = Physics2D.CircleCast(origin, circleCastRadius, Vector2.down, 1000f, groundLayer);

        Debug.DrawLine(origin, hit.point);

        return hit.distance <= groundedDistance;
    }

    public void Move(float direction)
    {
        oldDirection = inputDirection;

        //Set the target move speed (actual movement is handled in FixedUpdate)
        //Make direction either -1, 1 or 0 - no in-between
        if (GameManager.instance.gameRunning && canMove)
            inputDirection = direction != 0 ? Mathf.Sign(direction) : 0;
        else
            inputDirection = 0;

        if (inputDirection != oldDirection && OnChangedDirection != null && inputDirection != 0)
            OnChangedDirection(inputDirection);
    }

    public void Jump(bool pressed)
    {
        if (GameManager.instance.gameRunning && canMove)
        {
            if (pressed && !heldJump)
            {
                pressedJump = true;
                heldJump = true;
                jumpHeldTime = 0;
            }
            else
            {
                heldJump = false;
            }
        }
    }
}
