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

    public bool IsDashing { get; private set; }

    protected override float MoveSpeedMultiplier
    {
        get { return IsDashing ? dashSpeedMultiplier : 1.0f; }
    }

    protected override void Awake()
    {
        base.Awake();

        playerStats = GetComponent<PlayerStats>();
    }

    public bool Move(float direction, bool shouldDash)
    {
        // Only do dash and mana drain if we can move
        if (Move(direction))
        {
            IsDashing = shouldDash;

            if (IsDashing)
            {
                if (direction == 0 || playerStats.CurrentMana <= 0)
                {
                    dashManaDrain = 0;
                    IsDashing = false;
                }
                else
                {
                    dashManaDrain += dashManaDrainSpeed * Time.deltaTime;
                    if (dashManaDrain > 1.0f)
                    {
                        playerStats.RemoveMana(1);
                        dashManaDrain = dashManaDrain % 1;
                    }
                }
            }

            return true;
        }

        return false;
    }
}
