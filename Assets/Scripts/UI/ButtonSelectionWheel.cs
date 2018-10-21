using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ButtonSelectionWheel : MonoBehaviour
{
	private enum Direction
	{
		Up, Down, Left, Right
	}

	private enum ButtonDisplayTypes
	{
		Keyboard,
		PS4,
		XBOX
	}

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

	[SerializeField, ArrayForEnum(typeof(Direction))]
	private ElementManager.Element[] directionMappings;

    [SerializeField, ArrayForEnum(typeof(ButtonDisplayTypes))]
    private GameObject[] deviceButtonPrompts;

    private PlayerActions actions;
	private PlayerInput playerInput;
	private PlayerAttack playerAttack;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref directionMappings, typeof(Direction));
        ArrayForEnumAttribute.EnsureArraySize(ref deviceButtonPrompts, typeof(ButtonDisplayTypes));
    }

	private void Awake()
	{
		OnValidate();
	}

	private void Start()
	{
		actions = ControlManager.GetPlayerActions();
		button = actions?.GetButtonAction(holdButton);

        followTarget = GameManager.instance.player.transform;
		playerInput = GameManager.instance.player.GetComponent<PlayerInput>();
		playerAttack = GameManager.instance.player.GetComponent<PlayerAttack>();

        openClose.PreClose();
	}

	private void Update()
	{
		if (GameManager.instance.CanDoActions)
		{
			if (isOpen)
			{
				if (actions.ActiveDevice.Action1.WasPressed)
					SelectDirection(Direction.Down);
				else if (actions.ActiveDevice.Action2.WasPressed)
					SelectDirection(Direction.Right);
				else if (actions.ActiveDevice.Action3.WasPressed)
					SelectDirection(Direction.Left);
				else if (actions.ActiveDevice.Action4.WasPressed)
					SelectDirection(Direction.Up);
			}

			if (button != null)
			{
				switch (actions.LastInputType)
				{
					// When using a controller only keep open while button is pressed
                    case BindingSourceType.DeviceBindingSource:
						if (button.WasPressed)
							Open();
						else if (button.WasReleased)
							Close();
                        break;

					// When using a keyboard button is a toggle
					case BindingSourceType.KeyBindingSource:
						if (button.WasPressed)
						{
							if (!isOpen)
                                Open();
							else
                                Close();
                        }
                        break;
                }
			}
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
		if (isOpen)
			return;
		isOpen = true;

        UpdateButtonPrompts();

        playerInput.AcceptingInput = PlayerInput.InputAcceptance.MovementOnly;
		InteractManager.CanInteract = false;

		openClose.PlayOpen();
	}

	private void Close()
	{
		if (!isOpen)
			return;
		isOpen = false;

		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
		InteractManager.CanInteract = true;

		openClose.PlayClose();
	}

	private void SelectDirection(Direction direction)
	{
		playerInput.SkipFrame = true;
		playerAttack.SetSelectedMagic(directionMappings[(int)direction]);

		Close();
	}

	private void UpdateButtonPrompts()
	{
		foreach(var obj in deviceButtonPrompts)
            obj?.SetActive(false);

        BindingSourceType sourceType = actions.LastInputType;
        ButtonDisplayTypes? buttonDisplay = null;

        if (sourceType == BindingSourceType.KeyBindingSource)
            buttonDisplay = ButtonDisplayTypes.Keyboard;
        else if (sourceType == BindingSourceType.DeviceBindingSource)
		{
			switch (actions.LastDeviceStyle)
			{
				case InputDeviceStyle.Xbox360:
				case InputDeviceStyle.XboxOne:
                    buttonDisplay = ButtonDisplayTypes.XBOX;
                    break;

				case InputDeviceStyle.PlayStation3:
				case InputDeviceStyle.PlayStation4:
                    buttonDisplay = ButtonDisplayTypes.PS4;
                    break;
            }
		}

		if (buttonDisplay == null)
		{
            Debug.LogError($"Couldn't match source type \"{sourceType}\" with style \"{actions.LastDeviceStyle}\" to button display", this);
            return;
        }

        GameObject displayObj = deviceButtonPrompts[(int)buttonDisplay.Value];
		if (displayObj)
            displayObj.SetActive(true);
		else
            Debug.LogError($"No button prompt assigned for \"{buttonDisplay.Value}\"", this);
    }
}
