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

    [Space()]
    public GameObject manaBar;

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

            LoadManaBars();
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
    }

    void LoadManaBars()
    {
        //Get attacks for easy referencing
        MagicAttack[] attacks = playerAttack.magicAttacks;

        //For each attack
        for (int i = 0; i < attacks.Length; i++)
        {
            //Create a mana bar from template
            GameObject obj = (GameObject)Instantiate(manaBar, manaBar.transform.parent);

            ManaBar bar = obj.GetComponent<ManaBar>();

            //Tell the mana bar what attack it is for
            bar.playerAttack = playerAttack;
            bar.representingAttack = i;

            //Reload the bar
            bar.Reload();
        }

        //Disable the template bar
        manaBar.SetActive(false);
    }
}
