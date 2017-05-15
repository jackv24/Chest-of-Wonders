using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance;

    //Store one PlayerActions here for easy control rebinding
    public PlayerActions playerActions;

    private void Awake()
    {
        instance = this;

        playerActions = new PlayerActions();
    }

    public static PlayerActions GetPlayerActions()
    {
        return instance.playerActions;
    }
}
