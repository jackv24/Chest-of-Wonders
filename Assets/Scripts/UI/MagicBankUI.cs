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

		//Start with vials hidden
		fireVial.gameObject.SetActive(false);
		grassVial.gameObject.SetActive(false);
		iceVial.gameObject.SetActive(false);
		windVial.gameObject.SetActive(false);
	}

	void UpdateUI(ElementManager.Element element)
	{
		MagicVial vial = null;
		int currentSouls = 0;

		//Get values for correct element
		switch(element)
		{
			case ElementManager.Element.Fire:
				vial = fireVial;
				currentSouls = bank.currentFireSouls;
				break;
			case ElementManager.Element.Grass:
				vial = grassVial;
				currentSouls = bank.currentGrassSouls;
				break;
			case ElementManager.Element.Ice:
				vial = iceVial;
				currentSouls = bank.currentIceSouls;
				break;
			case ElementManager.Element.Wind:
				vial = windVial;
				currentSouls = bank.currentWindSouls;
				break;
			default:
				Debug.LogWarning("Magic Bank UI updating nonexistent element! Please make sure code is updated to reflect any new elements.");
				break;
		}

		//Will only fail if a new element is encountered
		if(vial != null)
		{
			StartCoroutine(UpdateVial(vial, currentSouls));
		}
	}

	IEnumerator UpdateVial(MagicVial vial, int currentSouls)
	{
		//Get animator if this vial hasn't already gotten it
		if (!vial.animator)
			vial.animator = vial.gameObject.GetComponent<Animator>();

		//Enable vial, automatically playing it's open animation
		vial.gameObject.SetActive(true);

		yield return new WaitForSeconds(openAnim.length);

		//Update with new values
		if (vial.slider)
			vial.slider.value = (float)currentSouls / bank.maxSouls;

		if (vial.amountText)
			vial.amountText.text = string.Format(amountText, currentSouls, bank.maxSouls);

		//Pause, then play hid animation
		yield return new WaitForSeconds(waitTime);

		StartCoroutine(HideVial(vial));
	}

	IEnumerator HideVial(MagicVial vial)
	{
		vial.animator.Play(closeAnim.name);

		yield return new WaitForSeconds(closeAnim.length);

		vial.gameObject.SetActive(false);
	}
}
