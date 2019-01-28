using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance;

    //Store one PlayerActions here for easy control rebinding
    private PlayerActions playerActions;
    private InControl.InControlInputModule inputModule;

    private void Awake()
    {
        instance = this;

        playerActions = new PlayerActions();

        //Setup UI input module with correct control bindings
        inputModule = FindObjectOfType<InControl.InControlInputModule>();
        if (inputModule)
        {
            inputModule.MoveAction = playerActions.Move;

            inputModule.SubmitAction = playerActions.Submit;
            inputModule.CancelAction = playerActions.Back;
        }
    }

    public static PlayerActions GetPlayerActions()
    {
        return instance.playerActions;
    }

    public static ButtonDisplayTypes? GetButtonDisplayType()
    {
        return GetButtonDisplayType(GetPlayerActions());
    }

    public static ButtonDisplayTypes? GetButtonDisplayType(PlayerActions playerActions)
    {
        BindingSourceType sourceType = playerActions.LastInputType;
        ButtonDisplayTypes? buttonDisplay = null;

        if (sourceType == BindingSourceType.KeyBindingSource)
            buttonDisplay = ButtonDisplayTypes.Keyboard;
        else if (sourceType == BindingSourceType.DeviceBindingSource)
        {
            switch (playerActions.LastDeviceStyle)
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
            Debug.LogError($"Couldn't match source type \"{sourceType}\" with style \"{playerActions.LastDeviceStyle}\" to button display");

        return buttonDisplay;
    }
}

public enum ButtonDisplayTypes
{
    Keyboard,
    PS4,
    XBOX
}
