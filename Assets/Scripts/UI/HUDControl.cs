using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject player;

    [Space()]
    public Slider healthSlider;
    public Text healthText;
    private string healthTextString;

    [System.Serializable]
    public class ManaBar
    {
        public Image attackIcon;
        public Slider slider;
        public Text sliderText;
        public Image cooldownImage;

        [HideInInspector]
        public string sliderTextString;
    }
    [Space()]
    public ManaBar manaBar1;
    public ManaBar manaBar2;

    private CharacterStats playerStats;
    private PlayerAttack playerAttack;

    private void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player");

        if (player)
        {
            playerStats = player.GetComponent<CharacterStats>();
            playerAttack = player.GetComponent<PlayerAttack>();

            if (playerAttack)
            {
                manaBar1.sliderTextString = manaBar1.sliderText.text;
                manaBar2.sliderTextString = manaBar2.sliderText.text;

                LoadManaBars();

                playerAttack.OnUsedMagic += (int slot) => { StartCoroutine("Cooldown", slot); };
            }
        }

        //Cache strings for formatting
        if (healthText)
            healthTextString = healthText.text;
    }

    private void Update()
    {
        if (playerStats)
        {
            //Health bar
            if (healthSlider)
                healthSlider.value = (float)playerStats.currentHealth / playerStats.maxHealth;

            if (healthText)
                healthText.text = string.Format(healthTextString, playerStats.currentHealth, playerStats.maxHealth);
        }

        if (playerAttack)
        {
            //if there is a magic attack in the slot
            if (playerAttack.magicSlot1.attack)
            {
                //Update slider
                if (manaBar1.slider)
                    manaBar1.slider.value = (float)playerAttack.magicSlot1.currentMana / playerAttack.magicSlot1.attack.manaAmount;

                //Update slider text
                if (manaBar1.sliderText)
                    manaBar1.sliderText.text = string.Format(manaBar1.sliderTextString, playerAttack.magicSlot1.currentMana, playerAttack.magicSlot1.attack.manaAmount);
            }

            //if there is a magic attack in the slot
            if (playerAttack.magicSlot2.attack)
            {
                //Update slider
                if (manaBar2.slider)
                    manaBar2.slider.value = (float)playerAttack.magicSlot2.currentMana / playerAttack.magicSlot2.attack.manaAmount;

                //Update slider text
                if (manaBar2.sliderText)
                    manaBar2.sliderText.text = string.Format(manaBar2.sliderTextString, playerAttack.magicSlot2.currentMana, playerAttack.magicSlot2.attack.manaAmount);
            }
        }
    }

    //Loads display for mana bars
    void LoadManaBars()
    {
        //If there is an attack in the slot
        if (playerAttack.magicSlot1.attack)
        {
            //Set attack icon
            manaBar1.attackIcon.sprite = playerAttack.magicSlot1.attack.icon;
            //Show mana bar
            manaBar1.attackIcon.transform.parent.gameObject.SetActive(true);
        }
        else
            //If there is no attack in slot, hide bar
            manaBar1.attackIcon.transform.parent.gameObject.SetActive(false);

        //If there is an attack in the slot
        if (playerAttack.magicSlot2.attack)
        {
            //Set attack icon
            manaBar2.attackIcon.sprite = playerAttack.magicSlot2.attack.icon;

            //Show mana bar
            manaBar2.attackIcon.transform.parent.gameObject.SetActive(true);
        }
        else
            //If there is no attack in slot, hide bar
            manaBar2.attackIcon.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator Cooldown(int slot)
    {
        //Get appropriate slot and bar from slot number
        PlayerAttack.MagicSlot s = slot == 1 ? playerAttack.magicSlot1 : playerAttack.magicSlot2;
        ManaBar b = slot == 1 ? manaBar1 : manaBar2;

        //Loop for time
        float timeElapsed = 0;
        while(timeElapsed <= s.attack.cooldownTime)
        {
            //Set cooldown image fill amount
            b.cooldownImage.fillAmount = 1 - (timeElapsed / s.attack.cooldownTime);

            //Do with frame
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }

        //Make sure it's 0 at the end to prevent artifacts
        b.cooldownImage.fillAmount = 0;
    }
}
