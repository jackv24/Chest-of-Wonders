using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
	public TextMeshProUGUI interactText;

	[Space()]
	public string openText = "UI/INTERACT_OPEN";
	public string talkText = "UI/INTERACT_TALK";

	private void Reset()
	{
		interactText = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void ShowPrompt(InteractType interactType)
	{
		if(interactText)
		{
			string text = "Interact";

			switch(interactType)
			{
				case InteractType.Open:
					text = openText;
					break;
				case InteractType.Speak:
					text = talkText;
					break;
			}

			interactText.text = text.TryGetTranslation();
		}
	}

	public void HidePrompt()
	{
		gameObject.SetActive(false);
	}
}
