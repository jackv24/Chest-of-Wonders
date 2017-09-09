using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
	public static HUDControl Instance;

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
	private List<GameObject> mixNotches = new List<GameObject>();
	[Space()]
	public Sprite mixEmptyNotch;
	public Sprite mixFireNotch;
	public Sprite mixGrassNotch;
	public Sprite mixIceNotch;
	public Sprite mixWindNotch;
	[Space()]
	public Image currentMixMagic;
	[Space()]
	public Sprite mixFireGraphic;
	public Sprite mixGrassGraphic;
	public Sprite mixIceGraphic;
	public Sprite mixWindGraphic;

	[System.Serializable]
	public class Progression
	{
		public GameObject[] enableObjects;
		public GameObject[] disableObjects;

		public void Evaluate()
		{
			foreach (GameObject obj in enableObjects)
				obj.SetActive(true);

			foreach (GameObject obj in disableObjects)
				obj.SetActive(false);
		}
	}

	[Header("Unlock Progression")]
	public Progression basicProgression;
	public Progression halfProgression;
	public Progression fullProgression;

	private CharacterStats playerStats;
    private PlayerAttack playerAttack;

	private void Awake()
	{
		Instance = this;
	}

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
			if (playerAttack.mixMagics.Count > 0)
				value = (float)playerAttack.mixMagics[0].currentMana / playerAttack.maxMana;

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
			switch(playerAttack.currentMagicProgression)
			{
				case PlayerAttack.MagicProgression.Basic:
					basicProgression.Evaluate();
					break;
				case PlayerAttack.MagicProgression.Half:
					halfProgression.Evaluate();
					break;
				case PlayerAttack.MagicProgression.Full:
					fullProgression.Evaluate();
					break;
			}

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
				currentBaseMagic.color = Color.white;

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
					default:
						currentBaseMagic.sprite = null;
						currentBaseMagic.color = Color.clear;
						break;
				}
			}

			UpdateMixNotches();
		}
    }

	void UpdateMixNotches()
	{
		int mixNotchCount = 1;

		if(playerAttack)
			mixNotchCount = playerAttack.mixMagics.Count;

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

		if(playerAttack)
		{
			//Update mix notch display
			for(int i = 0; i < mixNotchCount; i++)
			{
				Image image = mixNotches[i].GetComponent<Image>();

				switch(playerAttack.mixMagics[i].element)
				{
					case ElementManager.Element.Fire:
						image.sprite = mixFireNotch;
						break;
					case ElementManager.Element.Grass:
						image.sprite = mixGrassNotch;
						break;
					case ElementManager.Element.Ice:
						image.sprite = mixIceNotch;
						break;
					case ElementManager.Element.Wind:
						image.sprite = mixWindNotch;
						break;
					default:
						image.sprite = mixEmptyNotch;
						break;
				}
			}

			//Show currently selected mix magic
			if (currentMixMagic)
			{
				if (playerAttack.mixMagics.Count > 0 && playerAttack.mixMagics[0].element != ElementManager.Element.None)
				{
					currentMixMagic.color = Color.white;

					switch (playerAttack.mixMagics[0].element)
					{
						case ElementManager.Element.Fire:
							currentMixMagic.sprite = mixFireGraphic;
							break;
						case ElementManager.Element.Grass:
							currentMixMagic.sprite = mixGrassGraphic;
							break;
						case ElementManager.Element.Ice:
							currentMixMagic.sprite = mixIceGraphic;
							break;
						case ElementManager.Element.Wind:
							currentMixMagic.sprite = mixWindGraphic;
							break;
					}
				}
				else
				{
					currentMixMagic.sprite = null;
					currentMixMagic.color = Color.clear;
				}
			}
		}
	}
}
