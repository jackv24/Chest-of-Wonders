using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// All pages to be displayed in the notebook should inherit this class
/// </summary>
public abstract class NotebookPageUI : MonoBehaviour
{
	public Button menuButton;
	private I2.Loc.Localize menuButtonLocalize;

	public void Setup(NotebookUI notebook, int index)
	{
		if (menuButton)
		{
			menuButtonLocalize = menuButton.GetComponentInChildren<I2.Loc.Localize>();

			menuButton.onClick.AddListener(() =>
			{
				notebook.SwitchTo(index);
			});
		}
	}

	public void Show(bool quickShow, string buttonPrefix)
	{
		SetButtonPrefix(buttonPrefix);

		Open(quickShow);
	}

	public void Hide(bool quickHide, string buttonPrefix)
	{
		SetButtonPrefix(buttonPrefix);

		Close(quickHide);
	}

	private void SetButtonPrefix(string prefix)
	{
		if (menuButtonLocalize)
		{
			menuButtonLocalize.TermPrefix = prefix;
			menuButtonLocalize.OnLocalize(true);
		}
	}

	//Leave it to the inheriting class to implement it's own open/close for animations, setup, etc
	protected abstract void Open(bool quickShow);
	protected abstract void Close(bool quickHide);
}