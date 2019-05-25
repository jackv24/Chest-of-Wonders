using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField]
    private CompoundSlider healthBar;

    [SerializeField]
    private float healthDrainTime = 0.1f;

    private Coroutine healthDrainRoutine;

    [SerializeField]
    private Image manaBar;

    [SerializeField]
    private float manaDrainTime = 0.1f;

    private Coroutine manaDrainRoutine;

	[Header("Magic")]
    [SerializeField]
    private Image fireNotch;

    [SerializeField]
    private Image grassNotch;

    [SerializeField]
    private Image iceNotch;

    [SerializeField]
    private Image windNotch;

	[Space()]
    [SerializeField]
    private Color notchDisabledTint = Color.black;

    [SerializeField]
    private Color notchDeselectedTint = Color.gray;

	[Space()]
    [SerializeField]
    private Image currentMagic;

	[Space()]
    [SerializeField]
    private Sprite fireGraphic;

    [SerializeField]
    private Sprite grassGraphic;

    [SerializeField]
    private Sprite iceGraphic;

    [SerializeField]
    private Sprite windGraphic;

	[Space()]
    [SerializeField]
    private GameObject magicSwitchAnim;

    [SerializeField]
    private GameObject magicNotchSwitchAnim;

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

    private GameObject player;

    private PlayerStats playerStats;
    private PlayerAttack playerAttack;
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player");

        if (player)
        {
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats)
            {
                playerStats.OnHealthUpdated += UpdateHealthDisplay;
                playerStats.OnManaUpdated += UpdateManaDisplay;
            }
        }
    }

	private void Start()
    {
        if (player)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack)
            {
                playerAttack.OnUpdateMagic += UpdateAttackSlots;
                UpdateAttackSlots();

				playerAttack.OnSwitchMagic += PlaySwitchAnim;

                //Reload magic UI display when attacks are loaded from save
                playerAttack.OnUpdateMagic += UpdateAttackSlots;
            }
        }

		GameManager.instance.OnPausedChange += (value) =>
		{
			animator?.Play(value ? "Hide" : "Show");
		};
    }

    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (!healthBar)
            return;

        healthBar.BarCount = maxHealth;
        float startValue = healthBar.Value;
        float newValue = (float)currentHealth / maxHealth;

        if (healthDrainRoutine != null)
            StopCoroutine(healthDrainRoutine);

        healthDrainRoutine = this.StartTimerRoutine(0, healthDrainTime, (time) =>
        {
            healthBar.Value = Mathf.Lerp(startValue, newValue, time);
        });
    }

    private void UpdateManaDisplay(int currentMana, int maxMana)
    {
        if (!manaBar)
            return;

        float startValue = manaBar.fillAmount;
        float newValue = (float)currentMana / maxMana;

        if (manaDrainRoutine != null)
            StopCoroutine(manaDrainRoutine);

        manaDrainRoutine = this.StartTimerRoutine(0, manaDrainTime, (time) =>
        {
            manaBar.fillAmount = Mathf.Lerp(startValue, newValue, time);
        });
    }

    //Loads display for mana bars
    private void UpdateAttackSlots()
    {
		if (playerAttack)
		{
			switch(playerAttack.CurrentMagicProgression)
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
				fireNotch.color = playerAttack.HasFireMagic ? notchDeselectedTint : notchDisabledTint;
			if (grassNotch)
				grassNotch.color = playerAttack.HasGrassMagic ? notchDeselectedTint : notchDisabledTint;
			if (iceNotch)
				iceNotch.color = playerAttack.HasIceMagic ? notchDeselectedTint : notchDisabledTint;
			if (windNotch)
				windNotch.color = playerAttack.HasWindMagic ? notchDeselectedTint : notchDisabledTint;

			//Show selected base magic
			if(currentMagic)
			{
				currentMagic.color = Color.white;

				switch(playerAttack.SelectedElement)
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

	void PlaySwitchAnim(ElementManager.Element newElement)
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
			switch(playerAttack.SelectedElement)
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

            magicNotchSwitchAnim.transform.position = pos;

			magicNotchSwitchAnim.SetActive(false);
			magicNotchSwitchAnim.SetActive(true);
		}
	}
}
