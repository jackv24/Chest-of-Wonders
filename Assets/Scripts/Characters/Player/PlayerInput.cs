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
        inputDirection = playerActions.Move;

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
            characterMove.Move(inputDirection.x);

            if (playerActions.Jump.WasPressed)
                characterMove.Jump(true);
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
                playerAttack.UseMagic(inputDirection);
            else if (playerActions.MagicSwitch.WasPressed)
                playerAttack.SwitchMagic();
        }
    }
}
