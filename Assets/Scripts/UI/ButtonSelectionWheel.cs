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
	private KeepWorldPosOnCanvas keepPos;
	private Transform followTarget;

	[SerializeField]
	private Vector2 followOffset;

	[SerializeField]
	private OpenCloseAnimator openClose;

	private bool isOpen;

	private PlayerActions actions;
	private PlayerInput playerInput;

	private void Start()
	{
		actions = ControlManager.GetPlayerActions();
		button = actions?.GetButtonAction(holdButton);

		followTarget = GameManager.instance.player.transform;

		playerInput = GameManager.instance.player.GetComponent<PlayerInput>();

		openClose.PreClose();
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
		InteractManager.CanInteract = false;

		openClose.PlayOpen();
	}

	private void Close()
	{
		isOpen = false;
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
		InteractManager.CanInteract = true;

		openClose.PlayClose();
	}
}
