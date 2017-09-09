using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject player;

    [Header("Stat Bars")]
    public Slider healthBar;
    public Slider manaBar;

    public float barLerpSpeed = 1f;

	[Header("Base Magic")]
	public Image baseFireNotch;
	public Image baseGrassNotch;
	public Image baseIceNotch;
	public Image baseWindNotch;
	[Space()]
	public Color baseNotchDisabledTint = Color.black;
	public Color baseNotchDeselectedTint = Color.gray;
	[Space()]
	public Image currentBaseMagic;
	[Space()]
	public Sprite baseFireGraphic;
	public Sprite baseGrassGraphic;
	public Sprite baseIceGraphic;
	public Sprite baseWindGraphic;

	[Header("Mix Magic")]
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
		if (playerAttack)
		{
			//Tint base magic notches to reflect obtained state
			if (baseFireNotch)
				baseFireNotch.color = playerAttack.baseFireObtained ? baseNotchDeselectedTint : baseNotchDisabledTint;
			if (baseGrassNotch)
				baseGrassNotch.color = playerAttack.baseGrassObtained ? baseNotchDeselectedTint : baseNotchDisabledTint;
			if (baseIceNotch)
				baseIceNotch.color = playerAttack.baseIceObtained ? baseNotchDeselectedTint : baseNotchDisabledTint;
			if (baseWindNotch)
				baseWindNotch.color = playerAttack.baseWindObtained ? baseNotchDeselectedTint : baseNotchDisabledTint;

			//Show selected base magic
			if(currentBaseMagic)
			{
				switch(playerAttack.baseMagicSelected)
				{
					case ElementManager.Element.Fire:
						currentBaseMagic.sprite = baseFireGraphic;
						baseFireNotch.color = Color.white;
						break;
					case ElementManager.Element.Grass:
						currentBaseMagic.sprite = baseGrassGraphic;
						baseGrassNotch.color = Color.white;
						break;
					case ElementManager.Element.Ice:
						currentBaseMagic.sprite = baseIceGraphic;
						baseIceNotch.color = Color.white;
						break;
					case ElementManager.Element.Wind:
						currentBaseMagic.sprite = baseWindGraphic;
						baseWindNotch.color = Color.white;
						break;
				}
			}

			UpdateMixNotches();
		}
    }

	void UpdateMixNotches()
	{
		///Make notch UI match amount of required notches
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

			//Remove unneeded notches (should never happen in normal gameplay, but good to have anyway)
			int notchesUnneeded = -notchesNeeded;
			int currentCount = mixNotches.Count;
			for(int i = 1; i <= notchesUnneeded; i++)
			{
				int index = currentCount - i;

				Destroy(mixNotches[index]);
				mixNotches.RemoveAt(index);
			}
		}
	}
}
