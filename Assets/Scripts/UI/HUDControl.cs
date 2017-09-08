using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject player;

    [Space()]
    public Slider healthBar;
    public Slider manaBar;

    public float barLerpSpeed = 1f;

	[Space()]
	public GameObject mixNotch;
	public int mixNotchCount = 4;
	private List<GameObject> mixNotches = new List<GameObject>();

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
                playerAttack.OnUpdateMagic += UpdateAttackSlots;

                UpdateAttackSlots();

                //Reload magic UI display when attacks are loaded from save
                if (GameManager.instance)
                    GameManager.instance.OnSaveLoaded += UpdateAttackSlots;
            }
        }
    }

    private void Update()
    {
        if (playerStats)
        {
            //Health bar
            if (healthBar)
                healthBar.value = Mathf.Lerp(healthBar.value, (float)playerStats.currentHealth / playerStats.maxHealth, barLerpSpeed * Time.deltaTime);
        }

        if (playerAttack)
        {
            float value = 0f;

            //CURRENT MANA

            //Update slider
            if (manaBar)
                manaBar.value = Mathf.Lerp(manaBar.value, value, barLerpSpeed * Time.deltaTime);
        }
    }

    //Loads display for mana bars
    void UpdateAttackSlots()
    {
		Debug.LogWarning("Update attack slots UI not implemented!");

		UpdateMixNotches();
    }

	void UpdateMixNotches()
	{
		if (mixNotch)
		{
			//Enable template notch for instantiation
			mixNotch.SetActive(true);

			//Add requred notches
			int notchesNeeded = mixNotchCount - mixNotches.Count;
			for (int i = 0; i < notchesNeeded; i++)
			{
				GameObject obj = Instantiate(mixNotch, mixNotch.transform.parent);

				mixNotches.Add(obj);
			}

			//Disable template notch
			mixNotch.SetActive(false);

			//Disable unneeded notches
			//for (int i = mixNotches.Count - 1; i >= notchesNeeded; i--)
			//{
			//	Destroy(mixNotches[i]);
			//	mixNotches.RemoveAt(i);
			//}
		}
	}
}
