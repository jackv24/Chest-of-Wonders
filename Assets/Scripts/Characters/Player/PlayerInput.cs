using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInput : MonoBehaviour
{
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

    private bool canMove = true;

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
        playerAttack = GetComponent<PlayerAttack>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();
    }

    private void Update()
    {
        //Get input from controllers and keyboard, clamped
        inputDirection = canMove ? playerActions.Move : Vector2.zero;

        //Apply deadzone
        if (Mathf.Abs(inputDirection.x) <= moveDeadZone.x)
            inputDirection.x = 0;
        if (Mathf.Abs(inputDirection.y) <= moveDeadZone.y)
            inputDirection.y = 0;

        if (characterStats && Debug.isDebugBuild)
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                characterStats.currentHealth = characterStats.maxHealth;
            }
        }

        if (characterMove)
        {
            //Move the player using the CharacterMove script
            if(GameManager.instance && GameManager.instance.CanDoActions)
                characterMove.Move(inputDirection.x);

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

        //Can only attack while game is running
        if (playerAttack && GameManager.instance.CanDoActions)
        {
			if (playerActions.MeleeAttack.WasPressed)
				playerAttack.UseMelee(true, inputDirection.y);
			else if (playerActions.MeleeAttack.WasReleased)
				playerAttack.UseMelee(false, inputDirection.y);
			else if (playerActions.MagicAttack.IsPressed)
				playerAttack.UseMagic();
			else if (playerActions.SwitchMagicBase.WasPressed)
				playerAttack.SwitchBaseMagic();

            playerAttack.UpdateAimDirection(inputDirection, playerActions.MagicAimDiagonal.IsPressed);

            if (playerActions.AbsorbMagic.WasPressed)
            {
                playerAttack.AbsorbMagic(true);
                canMove = false;
            }
            else if (playerActions.AbsorbMagic.WasReleased)
            {
                playerAttack.AbsorbMagic(false);
                canMove = true;
            }
        }
    }
}
