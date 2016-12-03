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

        if (aimIndicator)
        {
            if (GameManager.instance.canMove)
            {
                aimIndicator.gameObject.SetActive(true);

                //If using keyboard and mouse, aim at mouse
                if (playerActions.LastInputType == BindingSourceType.KeyBindingSource)
                    attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimIndicator.position;
                //If using a controller, use controller
                else
                {
                    //If right stick is being used, aim at right stick
                    if (InputManager.ActiveDevice.RightStick.Vector.magnitude > 0.1f)
                        attackDirection = InputManager.ActiveDevice.RightStick.Vector;
                    //Otherwise, aim in input direction
                    else if (inputDirection.magnitude > 0.1f)
                        attackDirection = inputDirection;
                }

                attackDirection.Normalize();

                //Rotate aim indicator to direction
                float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
                aimIndicator.rotation = Quaternion.Euler(0, 0, rotationZ + rotationOffset);
            }
            else
                aimIndicator.gameObject.SetActive(false);
        }

        if (characterAttack)
        {
            if (playerActions.Attack1.WasPressed)
                characterAttack.UsePrimary();
            else if (playerActions.Attack2.WasPressed)
                characterAttack.UseSecondary();
        }
    }
}
