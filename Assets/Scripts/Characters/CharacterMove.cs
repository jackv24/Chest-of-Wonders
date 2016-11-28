using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMove : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The horizontal move speed (m/s).")]
    public float moveSpeed = 2f;

    [Tooltip("The rate at which the character accelerates to reach the move speed.")]
    public float acceleration = 1f;

    private float inputDirection = 0f;
    public float InputDirection { get { return inputDirection; } }

    [Header("Jumping")]
    public float minJumpHeight = 2f;
    public float maxJumpHeight = 6f;

    private float gravity;

    public float jumpHoldTime = 1f;
    private float jumpHeldTime = 0;

    [Space()]
    [Tooltip("How long after the player leaves the ground until they can no longer jump (recommended to have this delay for platformers).")]
    public float stopJumpDelay = 0.02f;
    private float stopJumpTime;

    private bool pressedJump = false;
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

    private void Start()
    {
        gravity = body.gravityScale;
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

        if (pressedJump)
        {
            pressedJump = false;

            if (isGrounded || Time.time < stopJumpTime)
            {
                //Stop double jumping when grounded
                stopJumpTime = 0;

                //Calculate velocity required to reach min jump height
                moveVector.y = CalculateVelocityFromJumpHeight(minJumpHeight);
            }
        }

        if (heldJump && jumpHeldTime <= jumpHoldTime)
        {
            jumpHeldTime += Time.fixedDeltaTime;

            float minVel = CalculateVelocityFromJumpHeight(minJumpHeight);
            float maxVel = CalculateVelocityFromJumpHeight(maxJumpHeight);

            moveVector.y = Mathf.Lerp(minVel, maxVel, jumpHeldTime / jumpHoldTime);

            body.gravityScale = 0;
        }
        else
            body.gravityScale = gravity;

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
        //Set the target move speed (actual movement is handled in FixedUpdate)
        inputDirection = direction;
    }

    public void Jump(bool pressed)
    {
        if (pressed)
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

    private float CalculateVelocityFromJumpHeight(float height)
    {
        return Mathf.Sqrt(2f * height * -Physics2D.gravity.y * gravity);
    }
}
