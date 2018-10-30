using UnityEngine;
using InControl;

public abstract class ButtonSelectionWheel : MonoBehaviour
{
	protected enum Direction
	{
		Up, Down, Left, Right
	}

	private enum ButtonDisplayTypes
	{
		Keyboard,
		PS4,
		XBOX
	}

	protected abstract PlayerAction HoldButton { get; }

	[SerializeField]
	private KeepWorldPosOnCanvas keepPos;
	private Transform followTarget;

	[SerializeField]
	private Vector2 followOffset;

	[SerializeField]
	private OpenCloseAnimator openClose;

	private bool isOpen;

    [SerializeField, ArrayForEnum(typeof(ButtonDisplayTypes))]
    private GameObject[] deviceButtonPrompts;

    [SerializeField, ArrayForEnum(typeof(Direction))]
    private Animator[] sectionAnimators;

    private Direction? selectedDirection = null;

    protected PlayerActions Actions;
	private PlayerInput playerInput;

	protected virtual void OnValidate()
	{
        ArrayForEnumAttribute.EnsureArraySize(ref deviceButtonPrompts, typeof(ButtonDisplayTypes));
        ArrayForEnumAttribute.EnsureArraySize(ref sectionAnimators, typeof(Direction));
    }

	private void Awake()
	{
		OnValidate();
	}

	protected virtual void Start()
	{
		Actions = ControlManager.GetPlayerActions();

        followTarget = GameManager.instance.player.transform;
		playerInput = GameManager.instance.player.GetComponent<PlayerInput>();

        openClose.PreClose();
	}

	private void Update()
	{
		if (GameManager.instance.CanDoActions)
		{
			if (isOpen)
			{
				if (Actions.SelectionWheelDown.WasPressed)
					SelectDirection(Direction.Down);
				else if (Actions.SelectionWheelRight.WasPressed)
					SelectDirection(Direction.Right);
				else if (Actions.SelectionWheelLeft.WasPressed)
					SelectDirection(Direction.Left);
				else if (Actions.SelectionWheelUp.WasPressed)
					SelectDirection(Direction.Up);

                if (Actions.SelectionWheelDown.WasReleased)
                    ConfirmDirection(Direction.Down);
                else if (Actions.SelectionWheelRight.WasReleased)
                    ConfirmDirection(Direction.Right);
                else if (Actions.SelectionWheelLeft.WasReleased)
                    ConfirmDirection(Direction.Left);
                else if (Actions.SelectionWheelUp.WasReleased)
                    ConfirmDirection(Direction.Up);
            }

			if (HoldButton.WasPressed)
			{
				if (!isOpen)
                    Open();
				else
                    Close();
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

        OnOpen();
        UpdateButtonPrompts();

        playerInput.AcceptingInput = PlayerInput.InputAcceptance.MovementOnly;
		InteractManager.CanInteract = false;

        int currentDirection = GetSelectedDirection();

        for (int i = 0; i < sectionAnimators.Length; i++)
        {
            sectionAnimators[i].SetBool("IsCurrent", i == currentDirection);
            sectionAnimators[i].SetBool("IsSelected", false);
        }

		openClose.PlayOpen();
	}

	private void Close()
	{
		if (!isOpen)
			return;
		isOpen = false;

        if (selectedDirection != null)
            ConfirmDirection(selectedDirection.Value);

		playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
		InteractManager.CanInteract = true;

        openClose.PlayClose();
	}

	private void SelectDirection(Direction direction)
	{
        if (selectedDirection != null)
            sectionAnimators[(int)selectedDirection].SetBool("IsSelected", false);

        selectedDirection = direction;

        if (selectedDirection != null)
            sectionAnimators[(int)selectedDirection].SetBool("IsSelected", true);
    }

    private void ConfirmDirection(Direction direction)
    {
        if (selectedDirection != direction)
            return;
        selectedDirection = null;

        if (DirectionConfirmed(direction))
        {
            playerInput.SkipFrame = true;

            Close();
        }
    }

	private void UpdateButtonPrompts()
	{
		foreach(var obj in deviceButtonPrompts)
            obj?.SetActive(false);

        BindingSourceType sourceType = Actions.LastInputType;
        ButtonDisplayTypes? buttonDisplay = null;

        if (sourceType == BindingSourceType.KeyBindingSource)
            buttonDisplay = ButtonDisplayTypes.Keyboard;
        else if (sourceType == BindingSourceType.DeviceBindingSource)
		{
			switch (Actions.LastDeviceStyle)
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
            Debug.LogError($"Couldn't match source type \"{sourceType}\" with style \"{Actions.LastDeviceStyle}\" to button display", this);
            return;
        }

        GameObject displayObj = deviceButtonPrompts[(int)buttonDisplay.Value];
		if (displayObj)
            displayObj.SetActive(true);
		else
            Debug.LogError($"No button prompt assigned for \"{buttonDisplay.Value}\"", this);
    }

    protected abstract bool DirectionConfirmed(Direction direction);

    protected virtual int GetSelectedDirection() { return -1; }
    protected virtual void OnOpen() { }
}
