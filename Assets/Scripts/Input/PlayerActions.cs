using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Jump;

    public PlayerTwoAxisAction Move;

    public PlayerAction MeleeAttack;
    public PlayerAction MagicAttack;
    public PlayerAction MagicSwitch;

    public PlayerAction AbsorbMagic;

    public PlayerAction Interact;
    public PlayerAction Submit;
    public PlayerAction Menu;
    public PlayerAction Pause;

    public PlayerActions()
    {
        //Create actions
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Jump = CreatePlayerAction("Jump");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        MeleeAttack = CreatePlayerAction("Melee Attack");
        MagicAttack = CreatePlayerAction("Magic Attack");
        MagicSwitch = CreatePlayerAction("Magic Switch");

        AbsorbMagic = CreatePlayerAction("Absorb Magic");

        Interact = CreatePlayerAction("Interact");
        Submit = CreatePlayerAction("Submit");
        Menu = CreatePlayerAction("Menu");
        Pause = CreatePlayerAction("Pause");

        //Bind actions
        //Movement
        Left.AddDefaultBinding(Key.A);
        Left.AddDefaultBinding(Key.LeftArrow);
        Left.AddDefaultBinding(InputControlType.DPadLeft);
        Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        Right.AddDefaultBinding(Key.D);
        Right.AddDefaultBinding(Key.RightArrow);
        Right.AddDefaultBinding(InputControlType.DPadRight);
        Right.AddDefaultBinding(InputControlType.LeftStickRight);

        Up.AddDefaultBinding(Key.W);
        Up.AddDefaultBinding(Key.UpArrow);
        Up.AddDefaultBinding(InputControlType.DPadUp);
        Up.AddDefaultBinding(InputControlType.LeftStickUp);

        Down.AddDefaultBinding(Key.S);
        Down.AddDefaultBinding(Key.DownArrow);
        Down.AddDefaultBinding(InputControlType.DPadDown);
        Down.AddDefaultBinding(InputControlType.LeftStickDown);

        Jump.AddDefaultBinding(Key.Space);
        Jump.AddDefaultBinding(InputControlType.Action1);

        //Attacking
        MeleeAttack.AddDefaultBinding(Key.J);
        MeleeAttack.AddDefaultBinding(InputControlType.Action3);

        MagicAttack.AddDefaultBinding(Key.K);
        MagicAttack.AddDefaultBinding(InputControlType.RightTrigger);
        MagicAttack.AddDefaultBinding(InputControlType.Action2);

        MagicSwitch.AddDefaultBinding(Key.Tab);
        MagicSwitch.AddDefaultBinding(InputControlType.RightBumper);

        AbsorbMagic.AddDefaultBinding(Key.Shift);
        AbsorbMagic.AddDefaultBinding(InputControlType.LeftTrigger);
        AbsorbMagic.AddDefaultBinding(InputControlType.LeftBumper);

        //Misc
        Interact.AddDefaultBinding(Key.E);
        Interact.AddDefaultBinding(InputControlType.Action4);

        Submit.AddDefaultBinding(Key.J);
        Submit.AddDefaultBinding(Key.E);
        Submit.AddDefaultBinding(Key.Space);
        Submit.AddDefaultBinding(Key.Return);
        Submit.AddDefaultBinding(InputControlType.Action1);
        Submit.AddDefaultBinding(InputControlType.Action4);

        Menu.AddDefaultBinding(Key.E);
        Menu.AddDefaultBinding(InputControlType.Action4);

        Pause.AddDefaultBinding(Key.Escape);
        Pause.AddDefaultBinding(InputControlType.Options);
    }
}
