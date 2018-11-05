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

    private bool isDashing;

    private PlayerStats playerStats;

    protected override void Awake()
    {
        base.Awake();

        playerStats = GetComponent<PlayerStats>();
    }

    protected override float MoveSpeedMultiplier
    {
        get { return isDashing ? dashSpeedMultiplier : 1.0f; }
    }

    public void Move(float direction, bool shouldDash)
    {
        isDashing = shouldDash;

        if (isDashing)
        {
            if (direction == 0 || playerStats.CurrentMana <= 0)
            {
                dashManaDrain = 0;
                isDashing = false;
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

        Move(direction);
    }
}
