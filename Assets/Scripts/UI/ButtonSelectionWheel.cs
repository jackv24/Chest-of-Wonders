using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ButtonSelectionWheel : MonoBehaviour
{
	[SerializeField]
	private PlayerActions.ButtonActionType holdButton;
	private PlayerAction button;

	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private KeepWorldPosOnCanvas keepPos;
	private Transform followTarget;

	[SerializeField]
	private Vector2 followOffset;

	[Space(), SerializeField]
	private Animator animator;

	[SerializeField]
	private string openAnim;

	[SerializeField]
	private string closeAnim;

	private bool isOpen;

	private PlayerActions actions;
	private PlayerInput playerInput;

	private void Start()
	{
		actions = ControlManager.GetPlayerActions();
		button = actions?.GetButtonAction(holdButton);

		followTarget = GameManager.instance.player.transform;

		playerInput = GameManager.instance.player.GetComponent<PlayerInput>();

		if (animator)
			animator.enabled = false;

		if (group)
			group.alpha = 0;
	}

	private void Update()
	{
		if (button != null && GameManager.instance.CanDoActions)
		{
			if (button.WasPressed)
				Open();
			else if (button.WasReleased)
				Close();
		}

		//if (isOpen)
			UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (!followTarget || !keepPos)
			return;

		keepPos.worldPos = (Vector2)followTarget.position + followOffset;
	}

	private void Open()
	{
		isOpen = true;
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.MovementOnly;

		if(animator)
		{
			animator.enabled = true;
			animator.Play(openAnim);
		}
	}

	private void Close()
	{
		isOpen = false;
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;

		if (animator)
			animator.Play(closeAnim);
	}
}
