using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicBankUI : MonoBehaviour
{
	[System.Serializable]
	public class MagicVial
	{
		public GameObject gameObject;
		[HideInInspector]
		public Animator animator;

		public Slider slider;
		public Text amountText;

		[HideInInspector]
		public int currentSouls;
	}

	public MagicVial fireVial;
	public MagicVial grassVial;
	public MagicVial iceVial;
	public MagicVial windVial;

	[Space()]
	public string amountText;

	[Space()]
	public AnimationClip openAnim;
	public AnimationClip closeAnim;
	public float waitTime = 1.0f;

	private PlayerMagicBank bank;

	private void Start()
	{
		GameObject player = GameObject.FindWithTag("Player");

		if(player)
		{
			bank = player.GetComponent<PlayerMagicBank>();

			if(bank)
			{
				bank.OnBankUpdate += UpdateUI;
			}
		}

		if(GameManager.instance)
		{
			//Update UI after loading game
			GameManager.instance.OnSaveLoaded += UpdateAll;

			//Subscribe to game pause events
			GameManager.instance.OnPausedChange += (bool paused) =>
			{
				List<MagicVial> vials = new List<MagicVial>();

				vials.Add(fireVial);
				vials.Add(grassVial);
				vials.Add(iceVial);
				vials.Add(windVial);

				//If game paused, show vials, else hide
				if (paused)
				{
					for (int i = 0; i < vials.Count; i++)
						StartCoroutine(ShowVial(vials[i], false));
				}
				else
				{
					for (int i = 0; i < vials.Count; i++)
						StartCoroutine(HideVial(vials[i]));
				}
			};
		}

		//Start with vials hidden
		fireVial.gameObject.SetActive(false);
		grassVial.gameObject.SetActive(false);
		iceVial.gameObject.SetActive(false);
		windVial.gameObject.SetActive(false);

		UpdateAll();
	}

	void UpdateAll()
	{
		for (int i = 0; i < System.Enum.GetNames(typeof(ElementManager.Element)).Length; i++)
			UpdateUI((ElementManager.Element)i);
	}

	void UpdateUI(ElementManager.Element element)
	{
		MagicVial vial = null;

		//Get values for correct element
		switch(element)
		{
			case ElementManager.Element.Fire:
				vial = fireVial;
				vial.currentSouls = bank.currentFireSouls;
				break;
			case ElementManager.Element.Grass:
				vial = grassVial;
				vial.currentSouls = bank.currentGrassSouls;
				break;
			case ElementManager.Element.Ice:
				vial = iceVial;
				vial.currentSouls = bank.currentIceSouls;
				break;
			case ElementManager.Element.Wind:
				vial = windVial;
				vial.currentSouls = bank.currentWindSouls;
				break;
			default:
				if(element != ElementManager.Element.None)
					Debug.LogWarning("Magic Bank UI updating nonexistent element! Please make sure code is updated to reflect any new elements.");
				break;
		}

		//Will only fail if a new element is encountered
		if(vial != null)
		{
			//Show magic vial, and auto hide
			StartCoroutine(ShowVial(vial, true));
		}
	}

	IEnumerator ShowVial(MagicVial vial, bool autoHide)
	{
		//Get animator if this vial hasn't already gotten it
		if (!vial.animator)
			vial.animator = vial.gameObject.GetComponent<Animator>();

		//Enable vial, automatically playing it's open animation
		vial.gameObject.SetActive(true);

		yield return new WaitForSecondsRealtime(openAnim.length);

		//Update with new values (if above zero)
		if (vial.slider)
			vial.slider.value = (float)vial.currentSouls / bank.maxSouls;

		if (vial.amountText)
			vial.amountText.text = string.Format(amountText, vial.currentSouls, bank.maxSouls);

		//Pause, then play hid animation
		yield return new WaitForSecondsRealtime(waitTime);

		if(autoHide)
			StartCoroutine(HideVial(vial));
	}

	IEnumerator HideVial(MagicVial vial)
	{
		vial.animator.Play(closeAnim.name);

		yield return new WaitForSecondsRealtime(closeAnim.length);

		vial.gameObject.SetActive(false);
	}
}
