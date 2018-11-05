using UnityEngine;
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
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private PlayerStats playerStats;
	private PlayerDodge playerDodge;
	private CharacterAnimator characterAnimator;

	public InputAcceptance AcceptingInput { get; set; }
	public bool SkipFrame { get; set; }

    private void Awake()
    {
        //Get references
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();
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
        if (playerStats)
        {
            if (Input.GetKeyDown(KeyCode.Equals))
                playerStats.AddHealth(1);
            if (Input.GetKeyDown(KeyCode.Minus))
                playerStats.RemoveHealth(1);

            if (Input.GetKeyDown(KeyCode.LeftBracket))
                playerStats.RemoveMana(10);
            if (Input.GetKeyDown(KeyCode.RightBracket))
                playerStats.AddMana(10);
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
			playerMove?.Move(inputDirection.x, playerActions.Dash.IsPressed);

		if (AcceptingInput != InputAcceptance.All)
			return;

		if(SkipFrame)
		{
			SkipFrame = false;
			return;
		}

		if (playerMove)
        {
            if (playerActions.Jump.WasPressed)
            {
                if (inputDirection.y < 0 && playerMove.IsOnPlatform)
                    playerMove.DropThroughPlatform();
                else
                    playerMove.Jump(true);
            }
            else if (playerActions.Jump.WasReleased)
                playerMove.Jump(false);
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
