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

    public PlayerAction Attack1;
    public PlayerAction Attack2;

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

        Attack1 = CreatePlayerAction("Primary Attack");
        Attack2 = CreatePlayerAction("Secondary Attack");

        Interact = CreatePlayerAction("Interact");
        Submit = CreatePlayerAction("Submit");
        Menu = CreatePlayerAction("Menu");
        Pause = CreatePlayerAction("Pause");

        //Bind actions
        Left.AddDefaultBinding(Key.A);
        Left.AddDefaultBinding(InputControlType.DPadLeft);
        Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        Right.AddDefaultBinding(Key.D);
        Right.AddDefaultBinding(InputControlType.DPadRight);
        Right.AddDefaultBinding(InputControlType.LeftStickRight);

        Up.AddDefaultBinding(Key.W);
        Up.AddDefaultBinding(InputControlType.DPadUp);
        Up.AddDefaultBinding(InputControlType.LeftStickUp);

        Down.AddDefaultBinding(Key.S);
        Down.AddDefaultBinding(InputControlType.DPadDown);
        Down.AddDefaultBinding(InputControlType.LeftStickDown);

        Jump.AddDefaultBinding(Key.Space);
        Jump.AddDefaultBinding(InputControlType.Action1);

        Attack1.AddDefaultBinding(Mouse.LeftButton);
        Attack1.AddDefaultBinding(Key.J);
        Attack1.AddDefaultBinding(InputControlType.Action3);

        Attack2.AddDefaultBinding(Mouse.RightButton);
        Attack2.AddDefaultBinding(Key.K);
        Attack2.AddDefaultBinding(InputControlType.Action2);

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
        Pause.AddDefaultBinding(InputControlType.Start);
    }
}
