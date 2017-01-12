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
    private CharacterAttack characterAttack;

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
        characterAttack = GetComponent<CharacterAttack>();
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
        if (characterAttack && GameManager.instance.gameRunning)
        {
            if (playerActions.Attack1.WasPressed)
                characterAttack.UsePrimary();
            else if (playerActions.Attack2.WasPressed)
                characterAttack.UseSecondary();
        }
    }
}
