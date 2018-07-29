using UnityEngine;
using TMPro;

/// <summary>
/// Highlights the heading if any of the sub-buttons are selected
/// </summary>
public class SubButtonHeading : MonoBehaviour
{
	public TextMeshProUGUI text;
	public Color selectedColor = Color.white;
	public Color deselectedColor = Color.grey;

	public float lerpSpeed = 2.0f;
	public bool useUnscaledTime = true;

	[Space()]
	public ButtonEventWrapper[] subButtons;

	private bool buttonSelected;

	private void Start()
	{
		foreach(ButtonEventWrapper button in subButtons)
		{
			button.OnDeselected += () => { buttonSelected = false; };
			button.OnSelected += () => { buttonSelected = true; };
		}
	}

	private void Update()
	{
		if(text)
		{
			text.color = Color.Lerp(
				text.color,
				buttonSelected ? selectedColor : deselectedColor,
				lerpSpeed * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
		}
	}
}
