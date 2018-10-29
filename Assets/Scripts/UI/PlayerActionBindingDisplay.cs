using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PlayerActionBindingDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI keyText;

    [SerializeField]
    private PlayerActions.ButtonActionType binding;

    private PlayerActions playerActions;

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        var bindings = playerActions.GetButtonAction(binding).Bindings;
        keyText.text = bindings.FirstOrDefault().Name;
    }
}
