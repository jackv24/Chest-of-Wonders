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
        Dash,
		MagicProjectileAttack,
		SwitchMagic,
		Dodge,
		Interact,
		Submit,
		Back,
		Pause,
        SelectionWheelUp,
        SelectionWheelDown,
        SelectionWheelLeft,
        SelectionWheelRight,
        ItemSelection
    }

    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Jump;

    public PlayerTwoAxisAction Move;

    public PlayerAction MeleeAttack;
	public PlayerAction MagicProjectileAttack;
    public PlayerAction MagicProjectileDiagonalLock;

    public PlayerAction Dash;

	public PlayerAction MagicSelection;
    public PlayerAction ItemSelection;

    public PlayerAction SelectionWheelUp;
	public PlayerAction SelectionWheelDown;
	public PlayerAction SelectionWheelLeft;
	public PlayerAction SelectionWheelRight;

    public PlayerAction Dodge;

	public PlayerAction Interact;
    public PlayerAction Submit;
    public PlayerAction Back;
    public PlayerAction Pause;

    public bool IsUsingKeyboard
    {
        get { return LastInputType == BindingSourceType.KeyBindingSource || LastInputType == BindingSourceType.MouseBindingSource; }
    }

    public PlayerActions()
    {
        CreateActions();
        AddDefaultKeyBindings();
        AddDefaultControllerBindings();
    }

    private void AddDefaultKeyBindings()
    {
        Left.AddDefaultBinding(Key.LeftArrow);
        Right.AddDefaultBinding(Key.RightArrow);
        Up.AddDefaultBinding(Key.UpArrow);
        Down.AddDefaultBinding(Key.DownArrow);

        Jump.AddDefaultBinding(Key.Z);

        MeleeAttack.AddDefaultBinding(Key.C);
        MagicProjectileAttack.AddDefaultBinding(Key.D);
        MagicProjectileDiagonalLock.AddDefaultBinding(Key.LeftShift);

        Dash.AddDefaultBinding(Key.LeftControl);

        MagicSelection.AddDefaultBinding(Key.S);
        ItemSelection.AddDefaultBinding(Key.A);

        SelectionWheelUp.AddDefaultBinding(Key.S);
        SelectionWheelDown.AddDefaultBinding(Key.X);
        SelectionWheelLeft.AddDefaultBinding(Key.Z);
        SelectionWheelRight.AddDefaultBinding(Key.C);

        Dodge.AddDefaultBinding(Key.X);

        Interact.AddDefaultBinding(Key.UpArrow);
        Interact.AddDefaultBinding(Key.DownArrow);

        Submit.AddDefaultBinding(Key.Z);
        Submit.AddDefaultBinding(Key.Return);

        Back.AddDefaultBinding(Key.X);
        Back.AddDefaultBinding(Key.Escape);

        Pause.AddDefaultBinding(Key.Escape);
        Pause.AddDefaultBinding(Key.Tab);
    }

    private void AddDefaultControllerBindings()
    {
        Left.AddDefaultBinding(InputControlType.DPadLeft);
        Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        Right.AddDefaultBinding(InputControlType.DPadRight);
        Right.AddDefaultBinding(InputControlType.LeftStickRight);

        Up.AddDefaultBinding(InputControlType.DPadUp);
        Up.AddDefaultBinding(InputControlType.LeftStickUp);

        Down.AddDefaultBinding(InputControlType.DPadDown);
        Down.AddDefaultBinding(InputControlType.LeftStickDown);

        Jump.AddDefaultBinding(InputControlType.Action1);

        MeleeAttack.AddDefaultBinding(InputControlType.Action3);
        MagicProjectileAttack.AddDefaultBinding(InputControlType.Action4);
        MagicProjectileDiagonalLock.AddDefaultBinding(InputControlType.LeftTrigger);

        Dash.AddDefaultBinding(InputControlType.Action2);

        MagicSelection.AddDefaultBinding(InputControlType.LeftBumper);
        ItemSelection.AddDefaultBinding(InputControlType.RightBumper);

        SelectionWheelUp.AddDefaultBinding(InputControlType.Action4);
        SelectionWheelDown.AddDefaultBinding(InputControlType.Action1);
        SelectionWheelLeft.AddDefaultBinding(InputControlType.Action3);
        SelectionWheelRight.AddDefaultBinding(InputControlType.Action2);

        Dodge.AddDefaultBinding(InputControlType.RightTrigger);

        Interact.AddDefaultBinding(InputControlType.Action2);

        Submit.AddDefaultBinding(InputControlType.Action1);
        Submit.AddDefaultBinding(InputControlType.Action4);

        Back.AddDefaultBinding(InputControlType.Action2);

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
        MagicProjectileAttack = CreatePlayerAction("Magic Projectile");
        MagicProjectileDiagonalLock = CreatePlayerAction("Diagonal Lock");

        Dash = CreatePlayerAction("Dash");

        MagicSelection = CreatePlayerAction("Switch Magic");
        ItemSelection = CreatePlayerAction("Use Item");

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
			return Interact.WasPressed && (Move.Vector.magnitude < Mathf.Epsilon || Mathf.Abs(Move.Y) > Mathf.Abs(Move.X));
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

			case ButtonActionType.Dash:
				return Dash;

			case ButtonActionType.MagicProjectileAttack:
				return MagicProjectileAttack;

			case ButtonActionType.SwitchMagic:
				return MagicSelection;

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

            case ButtonActionType.SelectionWheelUp:
                return SelectionWheelUp;

            case ButtonActionType.SelectionWheelDown:
                return SelectionWheelDown;

            case ButtonActionType.SelectionWheelLeft:
                return SelectionWheelLeft;

            case ButtonActionType.SelectionWheelRight:
                return SelectionWheelRight;

            case ButtonActionType.ItemSelection:
                return ItemSelection;
        }

		return null;
	}

    public string GetBoundButtonName(ButtonActionType actionType)
    {
        var action = GetButtonAction(actionType);

        if (action != null)
        {
            foreach(var binding in action.Bindings)
            {
                if (binding.BindingSourceType == LastInputType)
                    return binding.Name;
            }
        }

        return string.Empty;
    }
}
