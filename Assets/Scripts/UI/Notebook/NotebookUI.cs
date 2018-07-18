using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotebookUI : MonoBehaviour
{
	public NotebookPageUI[] notebookPages;
	private int selectedPage = -1;

	public string selectedButtonPrefix = "x ";
	public string deselectedButtonPrefix = "- ";

	private void Start()
	{
		for(int i = 0; i < notebookPages.Length; i++)
			notebookPages[i].Setup(this, i);

		if (notebookPages.Length > 0)
		{
			notebookPages[0].Show(true, selectedButtonPrefix);
			selectedPage = 0;
		}

		for (int i = 1; i < notebookPages.Length; i++)
			notebookPages[i].Hide(true, deselectedButtonPrefix);
	}

	private void OnEnable()
	{
		//Quick-show previously selected page (allowing navigation to be re-linked after game state changes)
		if (selectedPage >= 0)
			notebookPages[selectedPage].Show(true, selectedButtonPrefix);
	}

	/// <summary>
	/// Hides previous notebook page and shows new page
	/// </summary>
	/// <param name="index">The index of the page to show (will cause errors if out of bounds).</param>
	public void SwitchTo(int index)
	{
		if(selectedPage >= 0)
		{
			notebookPages[selectedPage].Hide(false, deselectedButtonPrefix);

			selectedPage = index;

			notebookPages[selectedPage].Show(false, selectedButtonPrefix);
		}
	}
}
