using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : CharacterMove
{
    [Header("Player Move")]
    [SerializeField]
    private float dashSpeedMultiplier = 1.5f;

    [SerializeField]
    private float dashManaDrainSpeed = 10.0f;
    private float dashManaDrain;
    private PlayerStats playerStats;

    [SerializeField]
    private int minStartDashMana = 10;
    private bool hasManaBlockedDash;

    protected override float MoveSpeedMultiplier
    {
        get { return IsDashing ? dashSpeedMultiplier : 1.0f; }
    }

    public bool IsDashing { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        playerStats = GetComponent<PlayerStats>();
    }

    public bool Move(float direction, bool shouldDash)
    {
        bool didMove = Move(direction);
        bool doMinManaTest = hasManaBlockedDash || (!IsDashing && shouldDash);

        // Only do dash and mana drain if we can move
        if (didMove)
        {
            IsDashing = shouldDash;

            if (IsDashing)
            {
                // Can only START dashing if mana is above a certain amount (but if already dashing can continue to drain until run out)
                bool blockDash = doMinManaTest && playerStats.CurrentMana < minStartDashMana;

                if (direction == 0 || playerStats.CurrentMana <= 0 || blockDash)
                {
                    dashManaDrain = 0;
                    IsDashing = false;
                    hasManaBlockedDash = true;
                }
                else
                {
                    hasManaBlockedDash = false;
                    dashManaDrain += dashManaDrainSpeed * Time.deltaTime;
                    if (dashManaDrain > 1.0f)
                    {
                        playerStats.RemoveMana(1);
                        dashManaDrain = dashManaDrain % 1;
                    }
                }
            }
        }

        return didMove;
    }
}
