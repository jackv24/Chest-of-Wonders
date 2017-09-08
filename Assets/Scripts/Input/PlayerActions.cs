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
    public PlayerAction MagicAimDiagonal;

    public PlayerAction AbsorbMagic;

    public PlayerAction Interact;
    public PlayerAction Submit;
    public PlayerAction Back;
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
        MagicAimDiagonal = CreatePlayerAction("Magic Aim Diagonal");

        AbsorbMagic = CreatePlayerAction("Absorb Magic");

        Interact = CreatePlayerAction("Interact");
        Submit = CreatePlayerAction("Submit");
        Back = CreatePlayerAction("Back");
        Pause = CreatePlayerAction("Pause");

        //Bind actions
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

        Jump.AddDefaultBinding(Key.Space);
		Jump.AddDefaultBinding(Key.C);
		Jump.AddDefaultBinding(InputControlType.Action1);

        //Attacking
        MeleeAttack.AddDefaultBinding(Key.Z);
        MeleeAttack.AddDefaultBinding(InputControlType.Action3);

        MagicAttack.AddDefaultBinding(Key.X);
        MagicAttack.AddDefaultBinding(InputControlType.Action2);

        AbsorbMagic.AddDefaultBinding(Key.A);
        AbsorbMagic.AddDefaultBinding(InputControlType.LeftTrigger);
        AbsorbMagic.AddDefaultBinding(InputControlType.LeftBumper);

        MagicAimDiagonal.AddDefaultBinding(Key.LeftShift);
        MagicAimDiagonal.AddDefaultBinding(InputControlType.RightTrigger);

        //Misc
        Interact.AddDefaultBinding(Key.UpArrow);
        Interact.AddDefaultBinding(InputControlType.Action4);

        Submit.AddDefaultBinding(Key.Z);
        Submit.AddDefaultBinding(Key.X);
		Submit.AddDefaultBinding(Key.C);
		Submit.AddDefaultBinding(Key.Space);
        Submit.AddDefaultBinding(Key.Return);
        Submit.AddDefaultBinding(InputControlType.Action1);
        Submit.AddDefaultBinding(InputControlType.Action4);

        Back.AddDefaultBinding(Key.Escape);
        Back.AddDefaultBinding(InputControlType.Action2);

        Pause.AddDefaultBinding(Key.Escape);
        Pause.AddDefaultBinding(InputControlType.Options);
    }
}
