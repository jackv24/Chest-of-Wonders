using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject player;

    [Space()]
    public Image healthSlider;
    public Text healthText;
    private string healthTextString;

    [System.Serializable]
    public class ManaBar
    {
        public Image attackIcon;
        public Image slider;
        public Text sliderText;
        public Image cooldownImage;

        [HideInInspector]
        public string sliderTextString;
    }
    [Space()]
    public ManaBar manaBar;

    [Space()]
    public float barLerpSpeed = 1f;

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
                if(manaBar.sliderText)
                    manaBar.sliderTextString = manaBar.sliderText.text;

                LoadManaBar();
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
                healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, (float)playerStats.currentHealth / playerStats.maxHealth, barLerpSpeed * Time.deltaTime);

            if (healthText)
                healthText.text = string.Format(healthTextString, playerStats.currentHealth, playerStats.maxHealth);
        }

        if (playerAttack)
        {
            //if there is a magic attack in the slot
            if (playerAttack.magicSlotSelected.attack)
            {
                //Update slider
                if (manaBar.slider)
                    manaBar.slider.fillAmount = Mathf.Lerp(manaBar.slider.fillAmount, (float)playerAttack.magicSlotSelected.currentMana / playerAttack.magicSlotSelected.attack.manaAmount, barLerpSpeed * Time.deltaTime);

                //Update slider text
                if (manaBar.sliderText)
                    manaBar.sliderText.text = string.Format(manaBar.sliderTextString, playerAttack.magicSlotSelected.currentMana, playerAttack.magicSlotSelected.attack.manaAmount);
            }
        }
    }

    //Loads display for mana bars
    void LoadManaBar()
    {
        if (manaBar.attackIcon)
        {
            //If there is an attack in the slot
            if (playerAttack.magicSlotSelected.attack)
            {
                //Set attack icon
                manaBar.attackIcon.sprite = playerAttack.magicSlotSelected.attack.icon;
                //Show mana bar
                manaBar.attackIcon.transform.parent.gameObject.SetActive(true);
            }
            else
                //If there is no attack in slot, hide bar
                manaBar.attackIcon.transform.parent.gameObject.SetActive(false);
        }
    }

    //IEnumerator Cooldown(int slot)
    //{
    //    //Get appropriate slot and bar from slot number
    //    PlayerAttack.MagicSlot s = slot == 1 ? playerAttack.magicSlot1 : playerAttack.magicSlot2;

    //    if (manaBar.cooldownImage)
    //    {
    //        //Loop for time
    //        float timeElapsed = 0;
    //        while (timeElapsed <= s.attack.cooldownTime)
    //        {
    //            //Set cooldown image fill amount
    //            manaBar.cooldownImage.fillAmount = 1 - (timeElapsed / s.attack.cooldownTime);

    //            //Do with frame
    //            yield return new WaitForEndOfFrame();
    //            timeElapsed += Time.deltaTime;
    //        }

    //        //Make sure it's 0 at the end to prevent artifacts
    //        manaBar.cooldownImage.fillAmount = 0;
    //    }
    //}
}
