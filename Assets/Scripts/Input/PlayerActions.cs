using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public enum ButtonActionType
	{
		Jump,
		MeleeAttack,
		MagicMeleeAttack,
		MagicProjectileAttack,
		SwitchMagic,
		Dodge,
		Interact,
		Submit,
		Back,
		Pause
	}

    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Jump;

    public PlayerTwoAxisAction Move;

    public PlayerAction MeleeAttack;
    public PlayerAction MagicMeleeAttack;
	public PlayerAction MagicProjectileAttack;

	public PlayerAction SwitchMagic;

    public PlayerAction SelectionWheelUp;
	public PlayerAction SelectionWheelDown;
	public PlayerAction SelectionWheelLeft;
	public PlayerAction SelectionWheelRight;

    public PlayerAction Dodge;

	public PlayerAction Interact;
    public PlayerAction Submit;
    public PlayerAction Back;
    public PlayerAction Pause;

    public PlayerActions()
    {
        CreateActions();
        AddDefaultBindings();
    }

    private void AddDefaultBindings()
    {
        //Movement
        Left.AddDefaultBinding(Key.LeftArrow);
        Left.AddDefaultBinding(InputControlType.DPadLeft);
        Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        Right.AddDefaultBinding(Key.RightArrow);
        Right.AddDefaultBinding(InputControlType.DPadRight);
        Right.AddDefaultBinding(InputControlType.LeftStickRight);

        Up.AddDefaultBinding(Key.UpArrow);
        Up.AddDefaultBinding(InputControlType.DPadUp);
        Up.AddDefaultBinding(InputControlType.LeftStickUp);

        Down.AddDefaultBinding(Key.DownArrow);
        Down.AddDefaultBinding(InputControlType.DPadDown);
        Down.AddDefaultBinding(InputControlType.LeftStickDown);

        Jump.AddDefaultBinding(Key.Z);
        Jump.AddDefaultBinding(InputControlType.Action1);

        //Attacking
        MeleeAttack.AddDefaultBinding(Key.C);
        MeleeAttack.AddDefaultBinding(InputControlType.Action3);

        MagicMeleeAttack.AddDefaultBinding(Key.V);
        MagicMeleeAttack.AddDefaultBinding(InputControlType.Action4);

        MagicProjectileAttack.AddDefaultBinding(Key.D);
        MagicProjectileAttack.AddDefaultBinding(InputControlType.Action2);

        SwitchMagic.AddDefaultBinding(Key.S);
        SwitchMagic.AddDefaultBinding(InputControlType.LeftBumper);

        SelectionWheelUp.AddDefaultBinding(Key.S);
        SelectionWheelUp.AddDefaultBinding(InputControlType.Action4);

        SelectionWheelDown.AddDefaultBinding(Key.X);
        SelectionWheelDown.AddDefaultBinding(InputControlType.Action1);

        SelectionWheelLeft.AddDefaultBinding(Key.Z);
        SelectionWheelLeft.AddDefaultBinding(InputControlType.Action3);

        SelectionWheelRight.AddDefaultBinding(Key.C);
        SelectionWheelRight.AddDefaultBinding(InputControlType.Action2);

        Dodge.AddDefaultBinding(Key.X);
        Dodge.AddDefaultBinding(InputControlType.RightTrigger);

        //Misc
        Interact.AddDefaultBinding(Key.UpArrow);
        Interact.AddDefaultBinding(Key.DownArrow);
        Interact.AddDefaultBinding(InputControlType.DPadUp);
        Interact.AddDefaultBinding(InputControlType.LeftStickUp);
        Interact.AddDefaultBinding(InputControlType.DPadDown);
        Interact.AddDefaultBinding(InputControlType.LeftStickDown);

        Submit.AddDefaultBinding(Key.Z);
        Submit.AddDefaultBinding(Key.Return);
        Submit.AddDefaultBinding(InputControlType.Action1);
        Submit.AddDefaultBinding(InputControlType.Action4);

        Back.AddDefaultBinding(Key.X);
        Back.AddDefaultBinding(Key.Escape);
        Back.AddDefaultBinding(InputControlType.Action2);

        Pause.AddDefaultBinding(Key.Escape);
        Pause.AddDefaultBinding(Key.Tab);
        Pause.AddDefaultBinding(InputControlType.Command);
    }

    private void CreateActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Jump = CreatePlayerAction("Jump");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        MeleeAttack = CreatePlayerAction("Melee Attack");

        MagicMeleeAttack = CreatePlayerAction("Physical Magic");

        MagicProjectileAttack = CreatePlayerAction("Magic Projectile");

        SwitchMagic = CreatePlayerAction("Switch Magic");

        SelectionWheelUp = CreatePlayerAction("Selection Wheel Up");
        SelectionWheelDown = CreatePlayerAction("Selection Wheel Down");
        SelectionWheelLeft = CreatePlayerAction("Selection Wheel Left");
        SelectionWheelRight = CreatePlayerAction("Selection Wheel Right");

        Dodge = CreatePlayerAction("Dodge");

        Interact = CreatePlayerAction("Interact");
        Submit = CreatePlayerAction("Submit");
        Back = CreatePlayerAction("Back");
        Pause = CreatePlayerAction("Pause");
    }

    public bool WasInteractPressed
	{
		get
		{
			// Interact.WasPressed returns true even when input is mostly X-based, so an additional check is needed
			return Interact.WasPressed && Mathf.Abs(Move.Y) > Mathf.Abs(Move.X);
		}
	}

	public PlayerAction GetButtonAction(ButtonActionType actionType)
	{
		switch(actionType)
		{
			case ButtonActionType.Jump:
				return Jump;

			case ButtonActionType.MeleeAttack:
				return MeleeAttack;

			case ButtonActionType.MagicMeleeAttack:
				return MagicMeleeAttack;

			case ButtonActionType.MagicProjectileAttack:
				return MagicProjectileAttack;

			case ButtonActionType.SwitchMagic:
				return SwitchMagic;

			case ButtonActionType.Dodge:
				return Dodge;

			case ButtonActionType.Interact:
				return Interact;

			case ButtonActionType.Submit:
				return Submit;

			case ButtonActionType.Back:
				return Back;

			case ButtonActionType.Pause:
				return Pause;
		}

		return null;
	}
}
