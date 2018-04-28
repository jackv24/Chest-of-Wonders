using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenUI : MonoBehaviour
{
	public static PauseScreenUI Instance;

	public Selectable[] menuButtons;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//Menu buttons are not linked to notebook until notebook calls link method
		LinkMenuButtons(null);
	}

	/// <summary>
	/// Intended to be called by things that may need to be linked to from the menu buttons, such as notebook pages
	/// </summary>
	/// <param name="rightSelectable">The selectable to select on pressing right from any pause screen buttons. Usually the first element of a notebook page.</param>
	public void LinkMenuButtons(Selectable rightSelectable)
	{
		List<Selectable> menuButtonsList = new List<Selectable>(menuButtons);

		for (int i = menuButtonsList.Count - 1; i >= 0; i--)
		{
			if (!menuButtonsList[i].CanSelect())
				menuButtonsList.RemoveAt(i);
		}

		//Link menu buttons
		for (int i = 0; i < menuButtonsList.Count; i++)
		{
			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.Explicit;

			//Link up to previous element (or wrap around to end if this is the first one)
			nav.selectOnUp = menuButtonsList[i > 0 ? (i - 1) : menuButtonsList.Count - 1];

			//Link up to next element (or wrap around to start if this is the last element
			nav.selectOnDown = menuButtonsList[i < menuButtonsList.Count - 1 ? (i + 1) : 0];

			//Link right to first inventory slot if it has an item in it (also check if there is a selectable)
			nav.selectOnRight = rightSelectable;

			menuButtonsList[i].navigation = nav;
		}
	}
}
