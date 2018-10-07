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

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (!followTarget || !keepPos)
			return;

		if (openClose.IsOpen)
		{
			if (!keepPos.enabled)
				keepPos.enabled = true;

			keepPos.worldPos = (Vector2)followTarget.position + followOffset;
		}
		else
		{
			if (keepPos.enabled)
				keepPos.enabled = false;
		}
	}

	private void Open()
	{
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.MovementOnly;
		InteractManager.CanInteract = false;

		openClose.PlayOpen();
	}

	private void Close()
	{
		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
		InteractManager.CanInteract = true;

		openClose.PlayClose();
	}
}
