using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
    // Need enumeration to match actions for easy access when not hard-coding to public fields
	public enum ButtonActionType
	{
        Left,
        Right,
        Up,
        Down,

        Jump,
        Dodge,

        MeleeAttack,
        MagicAttack,
        MagicDiagonalLock,

        CycleMagicLeft,
        CycleMagicRight,

        Interact,
        Submit,
        Back,
        Pause
    }

    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;

    public PlayerTwoAxisAction Move;

    public PlayerAction Jump;
    public PlayerAction Dodge;

    public PlayerAction MeleeAttack;
	public PlayerAction MagicAttack;
    public PlayerAction MagicDiagonalLock;

    public PlayerAction CycleMagicLeft;
    public PlayerAction CycleMagicRight;

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
        Dodge.AddDefaultBinding(Key.X);

        MeleeAttack.AddDefaultBinding(Key.C);
        MagicAttack.AddDefaultBinding(Key.D);
        MagicDiagonalLock.AddDefaultBinding(Key.LeftShift);

        CycleMagicLeft.AddDefaultBinding(Key.A);
        CycleMagicRight.AddDefaultBinding(Key.S);

        Interact.AddDefaultBinding(Key.UpArrow);
        Interact.AddDefaultBinding(Key.DownArrow);

        Submit.AddDefaultBinding(Key.Return);
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
        Dodge.AddDefaultBinding(InputControlType.Action2);

        MeleeAttack.AddDefaultBinding(InputControlType.Action4);
        MagicAttack.AddDefaultBinding(InputControlType.Action3);
        MagicDiagonalLock.AddDefaultBinding(InputControlType.RightTrigger);

        CycleMagicLeft.AddDefaultBinding(InputControlType.LeftBumper);
        CycleMagicRight.AddDefaultBinding(InputControlType.RightBumper);

        Interact.AddDefaultBinding(InputControlType.Action4);

        Submit.AddDefaultBinding(InputControlType.Action1);
        Back.AddDefaultBinding(InputControlType.Action2);

        Pause.AddDefaultBinding(InputControlType.Command);
    }

    private void CreateActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        Jump = CreatePlayerAction("Jump");
        Dodge = CreatePlayerAction("Dodge");

        MeleeAttack = CreatePlayerAction("Melee Attack");
        MagicAttack = CreatePlayerAction("Magic Projectile");
        MagicDiagonalLock = CreatePlayerAction("Diagonal Lock");

        CycleMagicLeft = CreatePlayerAction("Cycle Magic Left");
        CycleMagicRight = CreatePlayerAction("Cycle Magic Right");

        Interact = CreatePlayerAction("Interact");
        Submit = CreatePlayerAction("Submit");
        Back = CreatePlayerAction("Back");
        Pause = CreatePlayerAction("Pause");
    }

	public PlayerAction GetButtonAction(ButtonActionType actionType)
	{
		switch(actionType)
		{
            case ButtonActionType.Left:
                return Left;

            case ButtonActionType.Right:
                return Right;

            case ButtonActionType.Up:
                return Up;

            case ButtonActionType.Down:
                return Down;

            case ButtonActionType.Jump:
                return Jump;

            case ButtonActionType.Dodge:
                return Dodge;

            case ButtonActionType.MeleeAttack:
                return MeleeAttack;

            case ButtonActionType.MagicAttack:
                return MagicAttack;

            case ButtonActionType.MagicDiagonalLock:
                return MagicDiagonalLock;

            case ButtonActionType.CycleMagicLeft:
                return CycleMagicLeft;

            case ButtonActionType.CycleMagicRight:
                return CycleMagicRight;

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
