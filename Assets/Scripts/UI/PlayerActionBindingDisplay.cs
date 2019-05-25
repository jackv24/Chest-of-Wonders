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

    private void OnEnable()
    {
        if (playerActions == null)
            playerActions = ControlManager.GetPlayerActions();

        // Use text parsing system to get button display so it's consistent with buttons in dialogue
        if (keyText)
            keyText.text = TextHelper.GetButtonText(binding.ToString(), 22.4f); // 22.4f is (14 / 10) * 16 - specifically hard-coded for Picory font as it's too much work right now to work around TMP sprite scaling
    }
}
