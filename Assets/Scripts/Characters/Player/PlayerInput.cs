﻿using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInput : MonoBehaviour
{
	public enum InputAcceptance
	{
		All,
		None,
		MovementOnly
	}

    //Horizontal direction of the movement input
    private Vector2 inputDirection;

    //InControl active controller
    //private InputDevice device;
    private PlayerActions playerActions;

    public Vector2 moveDeadZone = new Vector2(0.1f, 0.05f);

    //Character scripts
    private CharacterMove characterMove;
    private PlayerAttack playerAttack;
    private CharacterStats characterStats;
	private PlayerDodge playerDodge;
	private CharacterAnimator characterAnimator;

	public InputAcceptance AcceptingInput { get; set; }
	public bool SkipFrame { get; set; }

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
        playerAttack = GetComponent<PlayerAttack>();
        characterStats = GetComponent<CharacterStats>();
		playerDodge = GetComponent<PlayerDodge>();
		characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();
    }

    private void Update()
    {
#if DEBUG
        if (characterStats)
        {
            if (Input.GetKeyDown(KeyCode.Equals))
                characterStats.AddHealth(1);
            if (Input.GetKeyDown(KeyCode.Minus))
                characterStats.RemoveHealth(1);
        }

        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale *= 0.5f;
        if (Input.GetKeyDown(KeyCode.Y))
            Time.timeScale = 1.0f;

#endif

        if (AcceptingInput != InputAcceptance.All)
		{
			if (playerActions.MeleeAttack.WasReleased)
			{
				playerAttack.SetHoldingBat(false);
			}
		}

		if (AcceptingInput == InputAcceptance.None)
			return;

        //Get input from controllers and keyboard, clamped
        inputDirection = playerActions.Move;

        //Apply deadzone
        if (Mathf.Abs(inputDirection.x) <= moveDeadZone.x)
            inputDirection.x = 0;
        if (Mathf.Abs(inputDirection.y) <= moveDeadZone.y)
            inputDirection.y = 0;

		//Move the player using the CharacterMove script
		if (GameManager.instance && GameManager.instance.CanDoActions)
			characterMove?.Move(inputDirection.x);

		if (AcceptingInput != InputAcceptance.All)
			return;

		if(SkipFrame)
		{
			SkipFrame = false;
			return;
		}

		if (characterMove)
        {
            if (playerActions.Jump.WasPressed)
            {
                if (inputDirection.y < 0 && characterMove.IsOnPlatform)
                    characterMove.DropThroughPlatform();
                else
                    characterMove.Jump(true);
            }
            else if (playerActions.Jump.WasReleased)
                characterMove.Jump(false);
        }

		if (GameManager.instance.CanDoActions)
		{
			if (characterAnimator)
			{
				characterAnimator.SetVerticalAxis(inputDirection.y);
			}

			if (playerAttack)
			{
				if (playerActions.MeleeAttack.WasPressed)
				{
					playerAttack.UseMelee(true, inputDirection.y);
				}
				else if (playerActions.MeleeAttack.WasReleased)
				{
					playerAttack.UseMelee(false, inputDirection.y);
				}
				else if (playerActions.MagicProjectileAttack.IsPressed)
				{
					playerAttack.UseProjectileMagic();
				}

				playerAttack.UpdateAimDirection(inputDirection, playerActions.MagicProjectileDiagonalLock.IsPressed);
			}

			if(playerDodge)
			{
				if (playerActions.Dodge.WasPressed)
					playerDodge.Dodge(inputDirection);
			}
		}
    }
}
