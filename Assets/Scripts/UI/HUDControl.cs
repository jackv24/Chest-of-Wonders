using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject player;

    [Space()]
    public Image healthBar;
    public Text healthText;
    private string healthTextString;

    [Space()]
    public Image manaBar;
    public Text manaText;
    private string manaTextString;

    public float barLerpSpeed = 1f;

    [System.Serializable]
    public class AttackSlot
    {
        public Image attackIcon;
        public Image cooldownImage;
    }
    [Space()]
    public AttackSlot primarySlot;
    public AttackSlot secondarySlot;

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
                if(manaText)
                    manaTextString = manaText.text;

                playerAttack.OnSwitchMagic += UpdateAttackSlots;

                UpdateAttackSlots();

                //Reload magic UI display when attacks are loaded from save
                if (GameManager.instance)
                    GameManager.instance.OnSaveLoaded += UpdateAttackSlots;

                StartCoroutine("UpdateCooldowns");
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
            if (healthBar)
                healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float)playerStats.currentHealth / playerStats.maxHealth, barLerpSpeed * Time.deltaTime);

            if (healthText)
                healthText.text = string.Format(healthTextString, playerStats.currentHealth, playerStats.maxHealth);
        }

        if (playerAttack)
        {
            //if there is a magic attack in the slot
            if (playerAttack.magicSlot1.attack)
            {
                //Update slider
                if (manaBar)
                    manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, (float)playerAttack.magicSlot1.currentMana / playerAttack.magicSlot1.attack.manaAmount, barLerpSpeed * Time.deltaTime);

                //Update slider text
                if (manaText)
                    manaText.text = string.Format(manaTextString, playerAttack.magicSlot1.currentMana, playerAttack.magicSlot1.attack.manaAmount);
            }
        }
    }

    //Loads display for mana bars
    void UpdateAttackSlots()
    {
        if (primarySlot.attackIcon)
        {
            //If there is an attack in the slot
            if (playerAttack.magicSlot1.attack)
            {
                //Set attack icon
                primarySlot.attackIcon.sprite = playerAttack.magicSlot1.attack.icon;
            }
        }

        if (secondarySlot.attackIcon)
        {
            //If there is an attack in the slot
            if (playerAttack.magicSlot2.attack)
            {
                //Set attack icon
                secondarySlot.attackIcon.sprite = playerAttack.magicSlot2.attack.icon;
            }
        }
    }

    IEnumerator UpdateCooldowns()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (playerAttack)
            {
                if (primarySlot.cooldownImage)
                {
                    primarySlot.cooldownImage.fillAmount = (playerAttack.magicSlot1.nextFireTime - Time.time) / playerAttack.magicSlot1.attack.cooldownTime;
                }

                if (secondarySlot.cooldownImage)
                {
                    secondarySlot.cooldownImage.fillAmount = (playerAttack.magicSlot2.nextFireTime - Time.time) / playerAttack.magicSlot2.attack.cooldownTime;
                }
            }
        }
    }
}
