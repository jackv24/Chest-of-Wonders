using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagicBank : MonoBehaviour
{
	public delegate void BankLoadedEvent();
	public event BankLoadedEvent OnBankLoaded;

	public delegate void ElementEvent(ElementManager.Element element);
	public event ElementEvent OnBankUpdate;

	public int maxSouls = 3;

	[Space()]
	public int currentFireSouls = 0;
	public int currentGrassSouls = 0;
	public int currentIceSouls = 0;
	public int currentWindSouls = 0;

	private void Start()
	{
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				maxSouls = data.MaxSouls;

				currentFireSouls = data.CurrentFireSouls;
				currentGrassSouls = data.CurrentGrassSouls;
				currentIceSouls = data.CurrentIceSouls;
				currentWindSouls = data.CurrentWindSouls;

				OnBankLoaded?.Invoke();
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				data.MaxSouls = maxSouls;

				data.CurrentFireSouls = currentFireSouls;
				data.CurrentGrassSouls = currentGrassSouls;
				data.CurrentIceSouls = currentIceSouls;
				data.CurrentWindSouls = currentWindSouls;
			};
		}
	}

	public void AddSoul(ElementManager.Element element)
	{
		//Increment correct element, clamped
		switch(element)
		{
			case ElementManager.Element.Fire:
				currentFireSouls = Mathf.Clamp(currentFireSouls + 1, 0, maxSouls);
				break;
			case ElementManager.Element.Grass:
				currentGrassSouls = Mathf.Clamp(currentGrassSouls + 1, 0, maxSouls);
				break;
			case ElementManager.Element.Ice:
				currentIceSouls = Mathf.Clamp(currentIceSouls + 1, 0, maxSouls);
				break;
			case ElementManager.Element.Wind:
				currentWindSouls = Mathf.Clamp(currentWindSouls + 1, 0, maxSouls);
				break;
		}

		//Call UI update events
		if(OnBankUpdate != null)
			OnBankUpdate(element);
	}
}
