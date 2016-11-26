using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMove : MonoBehaviour
{
    [Tooltip("The horizontal move speed (m/s).")]
    public float moveSpeed = 2f;
    private float targetMoveSpeed = 0f;

    [Tooltip("The rate at which the character accelerates to reach the move speed.")]
    public float acceleration = 1f;

    [Space()]
    public float minJumpHeight = 2f;
    public float maxJumpHeight = 6f;

    private bool pressedJump = false;

    //The vector to add to the velocity
    private Vector2 moveVector = Vector2.zero;
    //Public read-only property to get input 
    public Vector2 MoveVector { get { return moveVector; } }

    //The RigidBody2D attached to this GameObject
    private Rigidbody2D body;

    private void Awake()
    {
        //Get references
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Get current velocity
        moveVector = body.velocity;

        //Accelerate to reach target move speed
        moveVector.x = Mathf.Lerp(moveVector.x, targetMoveSpeed, acceleration * Time.fixedDeltaTime);

        if (pressedJump)
        {
            pressedJump = false;

            //Calculate velocity required to reach min jump height
            moveVector.y = Mathf.Sqrt(2f * minJumpHeight * -Physics2D.gravity.y * body.gravityScale);
        }

        body.velocity = moveVector;
    }

    public void Move(float direction)
    {
        //Set the target move speed (actual movement is handled in FixedUpdate)
        targetMoveSpeed = direction * moveSpeed;
    }

    public void Jump()
    {
        pressedJump = true;
    }
}
