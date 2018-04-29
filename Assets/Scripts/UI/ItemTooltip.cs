using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
	public static ItemTooltip Instance;

	public Text nameText;
	public Text descriptionText;

	public Vector2 offset = new Vector2(10, -10);

	private RectTransform rectTransform;

	private void Awake()
	{
		Instance = this;

		rectTransform = GetComponent<RectTransform>();
	}

	private void Start()
	{
		GameManager.instance.OnPausedChange += (bool isPaused) =>
		{
			if (isPaused)
				gameObject.SetActive(false);
		};

		gameObject.SetActive(false);
	}

	public void Show(string displayName, string description, Vector2 position)
	{
		rectTransform.position = position;
		rectTransform.anchoredPosition += offset;

		if (nameText)
			nameText.text = displayName;

		if (descriptionText)
			descriptionText.text = description;

		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
