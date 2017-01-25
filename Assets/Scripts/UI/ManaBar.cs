using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public PlayerAttack playerAttack;
    [HideInInspector]
    public int representingAttack = 0;

    private MagicAttack attack;

    [Space()]
    public Image attackIcon;
    public Slider slider;
    public Text sliderText;
    private string sliderTextString;

    [Space()]
    public GameObject selectedIndicator;

    void Start()
    {
        //Cache slider text for later formatting
        if (sliderText)
            sliderTextString = sliderText.text;

        //When magic is cycled, refresh bar
        if (playerAttack)
            playerAttack.OnCycleMagic += Reload;
    }

    void Update()
    {
        if(playerAttack)
        {
            //Set slider value
            slider.value = (float)playerAttack.manaAmounts[representingAttack] / attack.manaAmount;

            //Set values in text of slider
            sliderText.text = string.Format(sliderTextString, playerAttack.manaAmounts[representingAttack], attack.manaAmount);
        }
    }

    public void Reload()
    {
        //Update the attack that this bar represents
        attack = playerAttack.magicAttacks[representingAttack];

        //Update display
        attackIcon.sprite = attack.icon;

        //Show if selected or not
        if (selectedIndicator)
                selectedIndicator.SetActive(playerAttack.selectedMagic == representingAttack);
    }
}
