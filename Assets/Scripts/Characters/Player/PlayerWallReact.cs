using UnityEngine;
using System.Collections.Generic;

public class PlayerWallReact : MonoBehaviour
{
    [SerializeField]
    private float horizontalNormalThreshold = 0.9f;

    [SerializeField, Range(0, 1.0f)]
    private float horizontalRayHitPercent = 0.7f;

    [SerializeField]
    private float horizontalConsecutiveBonkDistance = 0.5f;

    private bool alreadyHitWall;
    private float lastXBonkPosition;

    private int wallBonkHash;
    private int roofBonkHash;

    private CharacterMove characterMove;
    private CharacterAnimator characterAnimator;
    private Animator animator;

    private void Awake()
    {
        characterMove = GetComponent<CharacterMove>();
        Debug.Assert(characterMove);

        characterAnimator = GetComponent<CharacterAnimator>();
        Debug.Assert(characterAnimator);
    }

    private void Start()
    {
        animator = characterAnimator.Animator;

        wallBonkHash = Animator.StringToHash("wallBonk");
        roofBonkHash = Animator.StringToHash("roofBonk");

        characterMove.OnChangedDirection += (dir) => alreadyHitWall = false;
    }

    private void LateUpdate()
    {
        // Get in LateUpdate after CharacterMove has populated arrays, so we can react in the same frame

        if (characterMove.InputDirection != 0)
            DetectWallBonk();

        if (!characterMove.IsGrounded)
            DetectRoofBonk();
    }

    private void DetectWallBonk()
    {
        // Can only bonk again if wall hit flag is reset or we have moved far enough
        if (alreadyHitWall && Mathf.Abs(transform.position.x - lastXBonkPosition) < horizontalConsecutiveBonkDistance)
            return;

        List<RaycastHit2D> horizontalRayHits = characterMove.HorizontalRaycastHits;
        int hitCount = 0;
        int rayCount = horizontalRayHits.Count;
        foreach (var rayHit in horizontalRayHits)
        {
            // If ray hit wall (normal is close enough to horizontal)
            if (rayHit.collider != null && Mathf.Abs(Vector2.Dot(Vector2.right, rayHit.normal)) >= horizontalNormalThreshold)
            {
                hitCount++;
            }
        }

        // Set bonk flags if any rays hit so we don't do bonk when sliding down wall
        if (hitCount > 0)
        {
            alreadyHitWall = true;
            lastXBonkPosition = transform.position.x;

            // Only do actual bonk if enough of our rays are hitting the wall
            if (hitCount >= Mathf.RoundToInt(rayCount * horizontalRayHitPercent))
                animator.SetTrigger(wallBonkHash);
        }
    }

    private void DetectRoofBonk()
    {
        List<RaycastHit2D> verticalRayHits = characterMove.VerticalRaycastHits;
        bool didHit = false;
        foreach (var rayHit in verticalRayHits)
        {
            if (rayHit.collider != null && rayHit.normal.y < 0)
            {
                didHit = true;
                break;
            }
        }

        if (didHit)
            animator.SetTrigger(roofBonkHash);
    }
}
