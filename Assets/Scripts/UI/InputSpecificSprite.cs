using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputSpecificSprite : MonoBehaviour
{
    public Sprite playstation;
    public Sprite xbox;
    public Sprite keyboard;

    private PlayerActions playerActions;

    private SpriteRenderer rend;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        //Create event handler for when the input type changes
        playerActions.OnLastInputTypeChanged += delegate
        {
            //Get name of current input device
            string device = InputManager.ActiveDevice.Name;

            //Set correct sprite for device name
            switch(device)
            {
                case "PlayStation 4 Controller":
                case "PlayStation 3 Controller":
                    rend.sprite = playstation;
                    break;
                case "XInput Controller":
                    rend.sprite = xbox;
                    break;
                default:
                    rend.sprite = keyboard;
                    break;
            }
        };
    }
}
