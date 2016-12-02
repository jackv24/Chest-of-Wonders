using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public CharacterStats targetStats;

    [Space()]
    public Slider healthSlider;
    public Text healthText;
    private string healthTextString;

    [Space()]
    public Slider manaSlider;
    public Text manaText;
    private string manaTextString;

    private void Start()
    {
        //Cache strings for formatting
        if(healthText)
            healthTextString = healthText.text;
        if (manaText)
            manaTextString = manaText.text;
    }

    private void Update()
    {
        if (targetStats)
        {
            //Health bar
            if (healthSlider)
                healthSlider.value = (float)targetStats.currentHealth / targetStats.maxHealth;

            if (healthText)
                healthText.text = string.Format(healthTextString, targetStats.currentHealth, targetStats.maxHealth);

            //Mana bar
            if (manaSlider)
                manaSlider.value = (float)targetStats.currentMana / targetStats.maxMana;

            if (manaText)
                manaText.text = string.Format(manaTextString, targetStats.currentMana, targetStats.maxMana);
        }
    }
}
