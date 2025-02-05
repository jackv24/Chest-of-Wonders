﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterMovementStates
{
	Normal,
	SetVelocity,
	Disabled
}

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMove : MonoBehaviour
{
    public delegate void DirectionChangeEvent(float newFloat);
    public event DirectionChangeEvent OnChangedDirection;

    public delegate void JumpEvent();
    public event JumpEvent OnJump;

	public delegate void GroundedEvent();
	public event GroundedEvent OnGrounded;

	public float FacingDirection { get; private set; } = 1;
    public bool CanChangeDirection { get; set; } = true;

    public bool HittingWall { get; private set; } = false;

    public List<RaycastHit2D> VerticalRaycastHits { get; private set; }
    public List<RaycastHit2D> HorizontalRaycastHits { get; private set; }

    [Header("Movement")]
    [Tooltip("The horizontal move speed (m/s).")]
    public float moveSpeed = 2f;
    //What fraction of the move speed to actually move at (move speed is dampened on slopes)
    private float slopeSpeedMultiplier = 1;

    protected virtual float MoveSpeedMultiplier { get { return 1.0f; } }

    [Tooltip("The rate at which the character accelerates to reach the move speed (m/s^2).")]
    public float acceleration = 1f;

    public Vector2 Velocity { get; set; }

    [Space()]
    [Tooltip("The maximum angle at which a slope is considered walkable upwards.")]
    public float upSlopeLimit = 50f;
    [Tooltip("The maximum angle at which a slope is considered walkable downwards (otherwise falling).")]
    public float downSlopeLimit = 30f;
    [Tooltip("The maximum distance at which a slope will be considered (prevents player snapping to slopes from great heights).")]
    public float maxSlopeDistance = 0.35f;
    [Tooltip("Should horizontal speed be dampened on slopes?")]
    public bool slopeSpeedDampening = true;

    public float InputDirection { get; private set; }

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

    public bool IsGrounded { get; private set; }
    private bool wasGrounded = true;

	public bool IsOnPlatform { get { return stickToPlatforms; } }

    private bool jumped = false;
    private bool shouldDetectPlatforms = false;
	private bool startDetectingPlatforms = false;

    public float platformDropDetectDelay = 0.2f;
    private float nonDetectPlatformsTime = 0;
    private bool stickToPlatforms = false;

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
    public float skinWidthX = 0.01f;
    public float skinWidthY = 0.01f;

    [Space()]
    public LayerMask groundLayer = 1 << 8;
    public LayerMask platformLayer = 1 << 13;

    [Space()]
    [Tooltip("When checked, prevents character from falling through ground on start.")]
    public bool startOnGround = true;
	public float startGroundDistance = 5.0f;

    [Header("Miscellaneous")]
    public float knockBackRecoveryTime = 1f;
    public bool allowKnockback = true;

    public CharacterMovementStates MovementState { get; set; }

	private Rigidbody2D body;

    private BoxCollider2D col;
    private Rect box;

    protected virtual void Awake()
    {
        //Get references
        col = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        canMove = true;

        if (body)
            body.isKinematic = true;

		if (startOnGround)
		{
			StartCoroutine(WaitForGround());
		}
	}

	IEnumerator WaitForGround()
	{
		bool hasHit = false;

		while (!hasHit)
		{
			//Fire ray from above ground, allowing feet to be penetrating a bit
			RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up, Vector2.down, 1000f, groundLayer);

			//Start on ground point from raycast
			if (hit.collider != null)
			{
				if(hit.distance <= startGroundDistance)
					transform.position = hit.point;

				hasHit = true;
			}

			yield return new WaitForEndOfFrame();
		}
	}

    private void EnsureRaycastListSize(List<RaycastHit2D> rayList, int size)
    {
        if (rayList.Count != size)
        {
            int difference = size - rayList.Count;
            if (difference > 0)
                rayList.AddRange(new RaycastHit2D[difference]);
            else
                rayList.RemoveRange(0, -difference);
        }
    }

    private void Update()
    {
        if (HorizontalRaycastHits == null)
            HorizontalRaycastHits = new List<RaycastHit2D>(horizontalRays);
        else
            HorizontalRaycastHits.Clear();

        if (VerticalRaycastHits == null)
            VerticalRaycastHits = new List<RaycastHit2D>(verticalRays);
        else
            VerticalRaycastHits.Clear();

        // Populate lists with blank RaycastHit2Ds
        EnsureRaycastListSize(HorizontalRaycastHits, horizontalRays);
        EnsureRaycastListSize(VerticalRaycastHits, verticalRays);

        // Only run if script should control movement
        if (MovementState == CharacterMovementStates.Disabled)
            return;

		//Store collider rect for easy typing
		box = Rect.MinMaxRect(
			col.bounds.min.x,
			col.bounds.min.y,
			col.bounds.max.x,
			col.bounds.max.y
			);

        //Apply gravity, capping fall speed
		if(MovementState != CharacterMovementStates.SetVelocity)
			Velocity = Velocity.Where(y: Mathf.Max(Velocity.y - gravity * Time.deltaTime, -maxFallSpeed));

        //Jumping (only if we're in a state that can jump)
		if(MovementState != CharacterMovementStates.SetVelocity)
        {
			if (IsGrounded)
			{
				stopJumpTime = Time.time + stopJumpDelay;
				startDetectingPlatforms = false;
			}
			else if (!startDetectingPlatforms)
			{
				startDetectingPlatforms = true;
				shouldDetectPlatforms = true;
			}

			//If jump button has been pressed
			if (shouldJump)
			{
				//Consume jump button flag
				shouldJump = false;

				jumped = true;
				shouldDetectPlatforms = false;

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

					stickToPlatforms = false;
				}
			}

            //If jump has been held for less than the max time
            if (heldJump && jumpHeldTime < jumpTime)
            {
                //Set velocity, slowly decreasing to zero over time
                Velocity = Velocity.Where(y: Mathf.Lerp(jumpForce, 0, jumpHeldTime / jumpTime));

                //Count time jump is held
                jumpHeldTime += Time.deltaTime;
            }
            else //If jump has been released, at can not be held again until a new jump is started
                jumpHeldTime = jumpTime;
        }
		else if (shouldJump) //Consume jump inputs even if we're not in a state that can jump
		{
			shouldJump = false;
		}

		//Vertical collision detection
		{
            //Calculate start and end points that rays will be cast from between
            Vector2 startPoint = new Vector2(box.xMin + skinWidthX, box.center.y);
            Vector2 endPoint = new Vector2(box.xMax - skinWidthX, box.center.y);

            //Distance of rays should (when cast from centre) extend past the bottom by velocity amount (or skin width if grounded)
            float distance = box.height / 2 + (IsGrounded ? skinWidthX : Mathf.Abs(Velocity.y * Time.deltaTime));

			//Rays cast downward should use either slope distance or velocity, whichever is larger (should prevent falling through ground when framerate is low)
			float slopeDistance = Mathf.Max((box.height / 2) + maxSlopeDistance, distance);

			//Not grounded unless a ray connects
			IsGrounded = false;

            //All raycasts
            bool connected = false;

            //Keep track of the shortest ray
            float minDistance = Mathf.Infinity;
            int index = 0;

            //Detect platforms if falling after jumped
            if(jumped && Velocity.y < 0)
            {
                jumped = false;
                shouldDetectPlatforms = true;
            }

            //Loops through and cast rays
            for(int i = 0; i < verticalRays; i++)
            {
                //Get origin between start and end points
                Vector2 origin = Vector2.Lerp(startPoint, endPoint, i / (float)(verticalRays - 1));

                //Cast ray
                if (Velocity.y > 0)
                    VerticalRaycastHits[i] = Physics2D.Raycast(origin, Vector2.up, distance, groundLayer);
                else
                {
                    if (shouldDetectPlatforms && Time.time > nonDetectPlatformsTime)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, slopeDistance, groundLayer | platformLayer);

                        if(hit.collider)
                        {
                            //Only actually count the hit if the character landed from above (don't jump up since rays are cast from centre)
                            if (hit.point.y < box.yMin - Velocity.y * Time.deltaTime || stickToPlatforms)
                            {
                                VerticalRaycastHits[i] = hit;

                                stickToPlatforms = true;
                            }
                        }

                        //Stop detecting platforms when touched non platform layer
                        if (VerticalRaycastHits[i].collider && ((1 << VerticalRaycastHits[i].collider.gameObject.layer) & platformLayer) == 0)
                        {
                            shouldDetectPlatforms = false;
                            stickToPlatforms = false;
                        }
                    }
                    else
                        VerticalRaycastHits[i] = Physics2D.Raycast(origin, Vector2.down, slopeDistance, groundLayer);
                }

                Debug.DrawLine(origin, new Vector2(origin.x, origin.y + Mathf.Sign(Velocity.y) * (Velocity.y > 0 ? distance : slopeDistance)));

                //If ray connected then player should be considered grounded
                if (VerticalRaycastHits[i].collider != null)
                {
                    connected = true;

                    //If this ray is shorter than any other, set it as the shortest and one to use
                    if (VerticalRaycastHits[i].distance < minDistance)
                    {
                        minDistance = VerticalRaycastHits[i].distance;
                        index = i;
                    }
                }
            }

            //If a ray has connected with the ground
            if (connected)
            {
                float angle = Vector2.Angle(Vector2.up, VerticalRaycastHits[index].normal);

                //If within distance, or if it should stick to the slope
                if (VerticalRaycastHits[index].distance <= distance || (angle <= downSlopeLimit && stickToSlope))
                {
                    //Set grounded and stop falling (if already falling)
                    if (Velocity.y <= 0)
                    {
                        IsGrounded = true;

						stickToSlope = true;

                        //Calculate speed dampening based on slope (if slope dampening is not desired, make a value of 1)
                        slopeSpeedMultiplier = slopeSpeedDampening ? Mathf.Cos(angle * Mathf.Deg2Rad) : 1;
                    }
                    else
                        //If moving upwards, stop jumping
                        heldJump = false;

                    Velocity = Velocity.Where(y: 0);

                    //Move player flush to ground (using shortest ray)
                    Vector2 delta = Vector2.down * (VerticalRaycastHits[index].distance - box.height / 2);

                    //If delta magnitude is below a very low threshold, zero it out (prevents slowly sinking/floating due to inaccuracies)
                    if (Mathf.Abs(delta.y) < 0.001f)
                        delta.y = 0;

                    transform.Translate(delta);
                }
                else
                    //Prevent character from snapping to ground when the slope was not within the limit
                    stickToSlope = false;
            }
        }

		if (MovementState != CharacterMovementStates.SetVelocity)
		{
			//Horizontal movement
			if ((canMove || ignoreCanMove))
                Velocity = Velocity.Where(x: Mathf.Lerp(Velocity.x, moveSpeed * slopeSpeedMultiplier * MoveSpeedMultiplier * InputDirection, acceleration * Time.deltaTime));
			else
                Velocity = Velocity.Where(x: Mathf.Lerp(Velocity.x, 0, acceleration * Time.deltaTime));
		}

        //Lateral collision detection
        {
            //Only required if there is lateral movement
            if (Velocity.x != 0)
            {
                //Start and end points that ray origins will lay between
                Vector2 startPoint = new Vector2(box.center.x, box.yMin + skinWidthY);
                Vector2 endPoint = new Vector2(box.center.x, box.yMax - skinWidthY);

                //Rays are cast out according to velocity, from the center
                float distance = box.width / 2 + Mathf.Abs(Velocity.x * Time.deltaTime);

                //Rays are cast in the direction of movement
                Vector2 direction = Velocity.x > 0 ? Vector2.right : Vector2.left;

				bool hitWall = false;

                //Cast required amount of rays
                for (int i = 0; i < horizontalRays; i++)
                {
                    //Calculate origin for this ray
                    Vector2 origin = Vector2.Lerp(startPoint, endPoint, i / (float)(horizontalRays - 1));

                    //Cast ray
                    RaycastHit2D hit = HorizontalRaycastHits[i] = Physics2D.Raycast(origin, direction, distance, groundLayer);

                    Debug.DrawLine(origin, new Vector2(origin.x + distance * direction.x, origin.y));

                    // If ray hit, there is a wall
                    // If we've already hit a wall then we just store the hit info above
                    if (!hitWall && hit.collider != null)
                    {
                        //Calculate angle of slope
                        float angle = Vector2.Angle(Vector2.up, hit.normal);

                        //This is only considered a wall if outside slope limit
                        if (angle > upSlopeLimit)
                        {
                            //Make character flush against wall
                            transform.Translate(direction * (hit.distance - box.width / 2));

                            //Cease any lateral movement
                            Velocity = Velocity.Where(x: 0);

                            hitWall = true;
                        }
                    }
                }

				HittingWall = hitWall;
            }
        }

		if (IsGrounded)
		{
			if (!wasGrounded)
			{
				wasGrounded = true;

				OnGrounded?.Invoke();
			}
		}
		else
		{
			wasGrounded = false;
		}

        if (IsGrounded || Mathf.Abs(Velocity.y) < 0.01f)
            Velocity = Velocity.Where(y: 0);

        if (Mathf.Abs(Velocity.x) < 0.01f)
            Velocity = Velocity.Where(x: 0);

        //Move character by velocity
        transform.Translate(Velocity * Time.deltaTime);
    }

    public bool Move(float direction)
    {
        if (MovementState != CharacterMovementStates.Normal)
            return false;

        //Update input direction
        if ((GameManager.instance.CanDoActions || ignoreCanMove) && (canMove || ignoreCanMove))
            InputDirection = direction < 0 ? Mathf.Floor(direction) : Mathf.Ceil(direction);
        else
            InputDirection = 0;

		//If direction has changed (and does not equal 0), then call changed direction event
		if (CanChangeDirection && InputDirection != FacingDirection && InputDirection != 0 && OnChangedDirection != null)
		{
			FacingDirection = InputDirection;

			OnChangedDirection(InputDirection);
		}

        return true;
    }

    public void SetFacing(float direction)
    {
        direction = Mathf.Sign(direction);
        if (FacingDirection != direction)
        {
            FacingDirection = direction;

            OnChangedDirection(direction);
        }
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

    public void DropThroughPlatform()
    {
        nonDetectPlatformsTime = Time.time + platformDropDetectDelay;

        stickToPlatforms = false;
    }

	public void ResetJump()
	{
		shouldJump = false;
		jumped = false;
		heldJump = false;
	}
}
