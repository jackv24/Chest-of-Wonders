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
        playerActions = new PlayerActions();
    }

    private void Update()
    {
        if(characterStats && Debug.isDebugBuild)
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                characterStats.currentHealth = characterStats.maxHealth;
            }
        }

        if (characterMove)
        {
            //Get active device at start of frame (always current)
            //device = InputManager.ActiveDevice;

            //Get input from controllers and keyboard, clamped
            inputDirection = playerActions.Move;

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
                playerAttack.UseMelee(true);
            else if (playerActions.MeleeAttack.WasReleased)
                playerAttack.UseMelee(false);
            else if (playerActions.MagicAttack.IsPressed)
                playerAttack.UseMagic(inputDirection);
            else if (playerActions.MagicSwitch.WasPressed)
                playerAttack.SwitchMagic();
        }
    }
}
