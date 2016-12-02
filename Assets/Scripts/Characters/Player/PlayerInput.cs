using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInput : MonoBehaviour
{
    public Transform aimIndicator;
    [Range(0, 360f)]
    public float rotationOffset;

    //Horizontal direction of the movement input
    private Vector2 inputDirection;
    private Vector3 attackDirection;

    //InControl active controller
    private InputDevice device;

    //Character scripts
    private CharacterMove characterMove;
    private CharacterAttack characterAttack;

    private void Awake()
    {
        //Get references
        characterMove = GetComponent<CharacterMove>();
        characterAttack = GetComponent<CharacterAttack>();
    }

    private void Update()
    {
        if (characterMove)
        {
            //Get active device at start of frame (always current)
            device = InputManager.ActiveDevice;

            //Get input from controllers and keyboard, clamped
            inputDirection = new Vector2(Mathf.Clamp(device.DPadX + device.LeftStickX, -1f, 1f), Mathf.Clamp(device.DPadY + device.LeftStickY, -1f, 1f));

            //Move the player using the CharacterMove script
            characterMove.Move(inputDirection.x);

            if (device.Action1.WasPressed || device.LeftBumper.WasPressed)
                characterMove.Jump(true);
            else if (device.Action1.WasReleased || device.LeftBumper.WasReleased)
                characterMove.Jump(false);
        }

        if (aimIndicator)
        {
            //If using keyboard and mouse, aim at mouse
            if(device.Name == "Keyboard/Mouse")
                attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimIndicator.position;
            //If using a controller, use controller
            else
            {
                //If right stick is being used, aim at right stick
                if (device.RightStick.Vector.magnitude > 0.1f)
                    attackDirection = new Vector2(device.RightStickX, device.RightStickY);
                //Otherwise, aim in input direction
                else if (inputDirection.magnitude > 0.1f)
                    attackDirection = inputDirection;
            }

            attackDirection.Normalize();

            //Rotate aim indicator to direction
            float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            aimIndicator.rotation = Quaternion.Euler(0, 0, rotationZ + rotationOffset);
        }

        if (characterAttack)
        {
            if (device.RightBumper.WasPressed)
                characterAttack.UsePrimary();
            else if (device.RightTrigger.WasPressed)
                characterAttack.UseSecondary();
        }
    }
}
