using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISounds : SoundEffectBase
{
    public SoundEffect buttonSelect;
    public SoundEffect buttonSubmit;

    void Start()
    {
        //Get all buttons in children
        Button[] buttons = GetComponentsInChildren<Button>(true);

        //Assign sound events to all buttons
        foreach(Button b in buttons)
        {
            ButtonEventWrapper events = b.gameObject.AddComponent<ButtonEventWrapper>();

            events.OnSelected += delegate { PlaySound(buttonSelect); };
            events.OnSubmitted += delegate { PlaySound(buttonSubmit); };
        }
    }
}
