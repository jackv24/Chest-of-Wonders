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

    public PlayerAction MagicMeleeAttack;

	public PlayerAction MagicProjectileAttack;
	public PlayerAction MagicAimDiagonal;

	public PlayerAction SwitchMagicLeft;
	public PlayerAction SwitchMagicRight;

	public PlayerAction Dodge;

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

		MagicMeleeAttack = CreatePlayerAction("Physical Magic");

		MagicProjectileAttack = CreatePlayerAction("Magic Projectile");
        MagicAimDiagonal = CreatePlayerAction("Magic Aim Diagonal");

		SwitchMagicLeft = CreatePlayerAction("Switch Base Left");
		SwitchMagicRight = CreatePlayerAction("Switch Mix Right");

		Dodge = CreatePlayerAction("Dodge");

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

        Jump.AddDefaultBinding(Key.Z);
		Jump.AddDefaultBinding(InputControlType.Action1);

        //Attacking
        MeleeAttack.AddDefaultBinding(Key.C);
        MeleeAttack.AddDefaultBinding(InputControlType.Action3);

        MagicMeleeAttack.AddDefaultBinding(Key.V);
        MagicMeleeAttack.AddDefaultBinding(InputControlType.Action4);

		MagicProjectileAttack.AddDefaultBinding(Key.D);
		MagicProjectileAttack.AddDefaultBinding(InputControlType.Action2);

		SwitchMagicLeft.AddDefaultBinding(Key.A);
		SwitchMagicLeft.AddDefaultBinding(InputControlType.LeftBumper);

		SwitchMagicRight.AddDefaultBinding(Key.S);
		SwitchMagicRight.AddDefaultBinding(InputControlType.RightBumper);

        //MagicAimDiagonal.AddDefaultBinding(Key.LeftShift);
        //MagicAimDiagonal.AddDefaultBinding(InputControlType.RightTrigger);

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
}
