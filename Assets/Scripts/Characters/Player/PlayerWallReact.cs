using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerWallReact : MonoBehaviour
{
    public Action OnWallJumped;

    private readonly int wallKickAnim = Animator.StringToHash("Wall Kick");
    private readonly int wallBonkGroundAnim = Animator.StringToHash("Wall Bump Ground");
    private readonly int wallBonkAirAnim = Animator.StringToHash("Wall Bump Air");
    private readonly int ceilingBonkAnim = Animator.StringToHash("Ceiling Bump");

    [SerializeField]
    private float horizontalNormalThreshold = 0.9f;

    [SerializeField, Range(0, 1.0f)]
    private float horizontalRayHitPercent = 0.7f;

    [SerializeField]
    private float horizontalConsecutiveBonkDistance = 0.5f;

    private bool alreadyHitWall;
    private bool alreadyHitRoof;
    private float lastXBonkPosition;

    [Space, SerializeField]
    private float wallBonkJumpWindow = 0.2f;
    private float wallBonkJumpTime = 0;

    [SerializeField]
    private Vector2 wallJumpVelocity = new Vector2(1, 1);

    [SerializeField]
    private AnimationCurve wallJumpVelocityCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [SerializeField]
    private float wallJumpDuration = 1.0f;
    private float wallJumpTimer;
    private bool isWallJumping;
    private float wallJumpDirection;

    [SerializeField]
    private CameraShakeTarget wallJumpCameraShake;

    [SerializeField]
    private CameraShakeTarget hardBonkCameraShake;

    [SerializeField]
    private SoundEventSingle wallBonkSound;

    [SerializeField]
    private SoundEventSingle roofBonkSound;

    [SerializeField]
    private SoundEventSingle wallJumpSound;

    private PlayerMove playerMove;
    private CharacterAnimator characterAnimator;
    private Animator animator;
    private PlayerDodge playerDodge;
    private PlayerActions playerActions;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        Debug.Assert(playerMove);

        characterAnimator = GetComponent<CharacterAnimator>();
        Debug.Assert(characterAnimator);

        playerDodge = GetComponent<PlayerDodge>();
        Debug.Assert(playerDodge);

        playerActions = ControlManager.GetPlayerActions();
    }

    private void Start()
    {
        animator = characterAnimator.Animator;

        playerMove.OnChangedDirection += (dir) => alreadyHitWall = false;
        playerMove.OnGrounded += () => alreadyHitRoof = false;
    }

    private void LateUpdate()
    {
        // Get in LateUpdate after CharacterMove has populated arrays, so we can react in the same frame

        bool didWallBonk = false;
        bool didRoofBonk = false;

        // If we're wall jumping then don't need to be holding direction, allows chaining wall jumps by just jumping
        if (playerMove.InputDirection != 0 || isWallJumping)
            didWallBonk = DetectWallBonk();

        if (!playerMove.IsGrounded)
            didRoofBonk = DetectRoofBonk();

        if (didWallBonk && playerDodge.IsDodging)
            hardBonkCameraShake.DoShake();
        else if (didRoofBonk && playerDodge.IsDodging)
            hardBonkCameraShake.DoShake();

        if (didWallBonk || didRoofBonk)
        {
            playerDodge.EndDodge(false);
            EndWallJump();
        }

        if (didWallBonk && !playerMove.IsGrounded)
            wallBonkJumpTime = Time.time + wallBonkJumpWindow;

        if (Time.time <= wallBonkJumpTime && playerActions.Jump.WasPressed)
            StartWallJump();
        else if (wallJumpTimer >= wallJumpDuration)
            EndWallJump();

        if (isWallJumping)
        {
            playerMove.Velocity = new Vector2(wallJumpVelocity.x * wallJumpDirection, wallJumpVelocity.y)
                * wallJumpVelocityCurve.Evaluate(wallJumpTimer / wallJumpDuration);

            wallJumpTimer += Time.deltaTime;
        }
    }

    private void StartWallJump()
    {
        if (isWallJumping)
            return;

        playerMove.MovementState = CharacterMovementStates.SetVelocity;
        playerMove.SetFacing(wallJumpDirection);
        isWallJumping = true;
        wallJumpTimer = 0;

        animator.Play(wallKickAnim);
        wallJumpCameraShake.DoShake();
        wallJumpSound.Play(transform.position, SoundType.Player);

        OnWallJumped?.Invoke();
    }

    private void EndWallJump()
    {
        if (!isWallJumping)
            return;

        playerMove.MovementState = CharacterMovementStates.Normal;
        isWallJumping = false;
    }

    private bool DetectWallBonk()
    {
        // Can only bonk again if wall hit flag is reset or we have moved far enough
        if (alreadyHitWall && Mathf.Abs(transform.position.x - lastXBonkPosition) < horizontalConsecutiveBonkDistance)
            return false;

        List<RaycastHit2D> horizontalRayHits = playerMove.HorizontalRaycastHits;
        int hitCount = 0;
        int rayCount = horizontalRayHits.Count;
        float rayNormalsXSum = 0;
        foreach (var rayHit in horizontalRayHits)
        {
            // If ray hit wall (normal is close enough to horizontal)
            if (rayHit.collider != null && Mathf.Abs(Vector2.Dot(Vector2.right, rayHit.normal)) >= horizontalNormalThreshold)
            {
                hitCount++;
                rayNormalsXSum += rayHit.normal.x;
            }
        }

        // Set bonk flags if any rays hit so we don't do bonk when sliding down wall
        if (hitCount > 0)
        {
            alreadyHitWall = true;
            lastXBonkPosition = transform.position.x;
            wallJumpDirection = Mathf.Sign(rayNormalsXSum);

            // Only do actual bonk if enough of our rays are hitting the wall
            if (hitCount >= Mathf.RoundToInt(rayCount * horizontalRayHitPercent))
            {
                animator.Play(playerMove.IsGrounded ? wallBonkGroundAnim : wallBonkAirAnim);
                wallBonkSound.Play(transform.position, SoundType.Player);

                return true;
            }
        }

        return false;
    }

    private bool DetectRoofBonk()
    {
        if (alreadyHitRoof)
            return false;

        List<RaycastHit2D> verticalRayHits = playerMove.VerticalRaycastHits;
        bool didHit = false;
        foreach (var rayHit in verticalRayHits)
        {
            if (rayHit.collider != null && rayHit.normal.y < 0)
            {
                didHit = true;
                alreadyHitRoof = true;
                break;
            }
        }

        if (didHit)
        {
            animator.Play(ceilingBonkAnim);
            roofBonkSound.Play(transform.position, SoundType.Player);
        }

        return didHit;
    }
}
