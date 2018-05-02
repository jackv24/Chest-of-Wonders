using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class EnemyJournalUICard : MonoBehaviour
{
	public Text nameText;
	public Text descriptionText;

	public Localize amountText;
	private string amountSuffix;

	public Image enemyImage;

	private void Start()
	{
		if (amountText)
			amountSuffix = amountText.TermSuffix;

		gameObject.SetActive(false);
	}

	public void Show(EnemyJournalRecord record)
	{
		gameObject.SetActive(true);

		if(nameText)
			nameText.text = record.displayName;

		if (descriptionText)
			descriptionText.text = record.description;

		if(amountText)
		{
			int amountKilled = EnemyJournalManager.Instance.GetKills(record);

			//Localised "defeated" text is displayed with amount as suffix
			amountText.TermSuffix = string.Format(amountSuffix, amountKilled);
			amountText.OnLocalize(true);
		}

		if(enemyImage)
		{
			enemyImage.sprite = record.cardImage;
			enemyImage.SetNativeSize();
		}
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
