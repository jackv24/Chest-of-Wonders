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

    public float barLerpSpeed = 1f;

	[Header("Magic")]
	public Image fireNotch;
	public Image grassNotch;
	public Image iceNotch;
	public Image windNotch;
	[Space()]
	public Color notchDisabledTint = Color.black;
	public Color notchDeselectedTint = Color.gray;
	[Space()]
	public Image currentMagic;
	[Space()]
	public Sprite fireGraphic;
	public Sprite grassGraphic;
	public Sprite iceGraphic;
	public Sprite windGraphic;
	[Space()]
	public GameObject magicSwitchAnim;
	public GameObject magicNotchSwitchAnim;
	private Vector2 magicNotchSwitchAnimOffset;

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

				//Get base notch switch offset from fire notch (first notch is reference)
				magicNotchSwitchAnimOffset = magicNotchSwitchAnim.transform.position - fireNotch.transform.position;

				playerAttack.OnSwitchMagic += PlaySwitchAnim;

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
    }

    //Loads display for mana bars
    void UpdateAttackSlots()
    {
		if (playerAttack)
		{
			switch(playerAttack.magicProgression)
			{
				case PlayerAttack.MagicProgression.Basic:
					basicProgression.Evaluate();
					break;
				case PlayerAttack.MagicProgression.Full:
					fullProgression.Evaluate();
					break;
			}

			//Tint base magic notches to reflect obtained state
			if (fireNotch)
				fireNotch.color = playerAttack.hasFireMagic ? notchDeselectedTint : notchDisabledTint;
			if (grassNotch)
				grassNotch.color = playerAttack.hasGrassMagic ? notchDeselectedTint : notchDisabledTint;
			if (iceNotch)
				iceNotch.color = playerAttack.hasIceMagic ? notchDeselectedTint : notchDisabledTint;
			if (windNotch)
				windNotch.color = playerAttack.hasWindMagic ? notchDeselectedTint : notchDisabledTint;

			//Show selected base magic
			if(currentMagic)
			{
				currentMagic.color = Color.white;

				switch(playerAttack.selectedElement)
				{
					case ElementManager.Element.Fire:
						currentMagic.sprite = fireGraphic;
						fireNotch.color = Color.white;
						break;
					case ElementManager.Element.Grass:
						currentMagic.sprite = grassGraphic;
						grassNotch.color = Color.white;
						break;
					case ElementManager.Element.Ice:
						currentMagic.sprite = iceGraphic;
						iceNotch.color = Color.white;
						break;
					case ElementManager.Element.Wind:
						currentMagic.sprite = windGraphic;
						windNotch.color = Color.white;
						break;
					default:
						currentMagic.sprite = null;
						currentMagic.color = Color.clear;
						break;
				}
			}
		}
    }

	void PlaySwitchAnim()
	{
		if (magicSwitchAnim)
		{
			//Disable before enabling to reset anim state when pressed too quickly
			magicSwitchAnim.SetActive(false);
			magicSwitchAnim.SetActive(true);
		}

		if (magicNotchSwitchAnim)
		{
			Vector3 pos = magicNotchSwitchAnim.transform.position;

			//Position switch anim on top of notch
			switch(playerAttack.selectedElement)
			{
				case ElementManager.Element.Fire:
					pos = fireNotch.transform.position;
					break;
				case ElementManager.Element.Grass:
					pos = grassNotch.transform.position;
					break;
				case ElementManager.Element.Ice:
					pos = iceNotch.transform.position;
					break;
				case ElementManager.Element.Wind:
					pos = windNotch.transform.position;
					break;
			}

			magicNotchSwitchAnim.transform.position = pos + (Vector3)magicNotchSwitchAnimOffset;

			magicNotchSwitchAnim.SetActive(false);
			magicNotchSwitchAnim.SetActive(true);
		}
	}
}
