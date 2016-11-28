using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(CharacterMove))]
public class PlayerInput : MonoBehaviour
{
    //Horizontal direction of the movement input
    private float inputDirection;

    //InControl active controller
    private InputDevice device;
    //The CharacterMove script attached to this GameObject
    private CharacterMove characterMove;

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
    }

    private void Update()
    {
        //Get active device at start of frame (always current)
        device = InputManager.ActiveDevice;

        //Get input from controllers and keyboard, clamped
        inputDirection = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + device.DPadX, -1f, 1f);

        //Move the player using the CharacterMove script
        characterMove.Move(inputDirection);

        if (Input.GetButtonDown("Jump") || device.Action1.WasPressed)
            characterMove.Jump(true);
        else if (Input.GetButtonUp("Jump") || device.Action1.WasReleased)
            characterMove.Jump(false);
    }
}
