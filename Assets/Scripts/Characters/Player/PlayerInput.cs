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

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Start()
    {
        playerActions = new PlayerActions();
    }

    private void Update()
    {
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
        if (playerAttack && GameManager.instance.gameRunning)
        {
            if (playerActions.MeleeAttack.WasPressed)
                playerAttack.UseMelee();
            else if (playerActions.MagicAttack.WasPressed)
                playerAttack.UseMagic();

            if (playerActions.CycleMagicLeft.WasPressed)
                playerAttack.CycleMagic(-1);
            else if (playerActions.CycleMagicRight.WasPressed)
                playerAttack.CycleMagic(1);
        }
    }
}
