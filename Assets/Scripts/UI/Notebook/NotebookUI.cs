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

public abstract class UIGridSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	private bool isSetup = false;

	public Selectable Selectable { get; protected set; }

	protected void Setup()
	{
		if (isSetup)
			return;
		isSetup = true;

		Selectable = GetComponent<Selectable>();
	}

	public abstract void OnSelect(BaseEventData eventData);
	public abstract void OnDeselect(BaseEventData eventData);

	public static void LinkNavigation(UIGridSlot[] slots, int rowSplit, Selectable menuButton)
	{
		//Treat 1D array as 2D array for easier traversal (left as list for display in inspector)
		int width = rowSplit;
		int height = slots.Length / rowSplit;

		//Loop horizontally for each row
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				Selectable self = Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j).Selectable;

				Navigation nav = new Navigation();

				//NOTE:	We only bother testing if slots are interactable to the right and down since the
				//		inventory fills up from the top-right corner
				if (self.CanSelect())
				{
					nav.mode = Navigation.Mode.Explicit;

					///Horizontal navigation
					//Left should be the slot on left, unless this is the leftmost slot, then left is first menu button (if it exists)
					nav.selectOnLeft = i > 0 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i - 1, j).Selectable : menuButton;

					//Right should be the slot on the right (clamped & only if interactable)
					Selectable slotRight = i < width - 1 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i + 1, j).Selectable : null;
					nav.selectOnRight = slotRight ? (slotRight.CanSelect() ? slotRight : null) : null;

					///Vertical navigation
					//Slot above (clamped)
					nav.selectOnUp = j > 0 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j - 1).Selectable : null;

					//Slot below (clamped & only if interactable)
					Selectable slotBelow = j < height - 1 ? Helper.Get1DArrayElementBy2DIndexes(slots, width, i, j + 1).Selectable : null;
					nav.selectOnDown = slotBelow ? (slotBelow.CanSelect() ? slotBelow : null) : null;

				}
				else
					nav.mode = Navigation.Mode.None;

				self.navigation = nav;
			}
		}
	}
}
