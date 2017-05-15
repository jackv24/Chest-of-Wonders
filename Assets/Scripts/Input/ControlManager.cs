using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
