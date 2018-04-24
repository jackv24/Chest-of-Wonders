using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenUI : MonoBehaviour
{
	[Header("Navigation")]
	public Selectable[] menuButtons;

	[Space()]
	public InventoryUI inventoryUI;

	public int inventoryRowSplit = 6;

	private void OnEnable()
	{
		///Re-link navigation on enable since things may have changed

		//Update inventory when pause screen is opened
		inventoryUI?.UpdateUI();

		//Get slots from already established list (default to empty array if not assigned to prevent errors)
		InventoryUISlot[] inventorySlots = inventoryUI?.slots ?? new InventoryUISlot[0];

		//Link menu buttons
		for(int i = 0; i < menuButtons.Length; i++)
		{
			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.Explicit;

			//Link up to previous element (or wrap around to end if this is the first one)
			nav.selectOnUp = menuButtons[i > 0 ? (i - 1) : menuButtons.Length - 1];

			//Link up to next element (or wrap around to start if this is the last element
			nav.selectOnDown = menuButtons[i < menuButtons.Length - 1 ? (i + 1) : 0];

			//Link right to first inventory slot if it has an item in it (also check if there is a selectable)
			nav.selectOnRight = inventorySlots.Length > 0 ? (inventorySlots[0].Selectable.IsInteractable() ? inventorySlots[0].Selectable : null) : null;

			menuButtons[i].navigation = nav;
		}

		//Treat 1D array as 2D array for easier traversal (left as list for display in inspector)
		int width = inventoryRowSplit;
		int height = inventorySlots.Length / inventoryRowSplit;

		//Loop horizontally for each row
		for(int j = 0; j < height; j++)
		{
			for(int i = 0; i < width; i++)
			{
				Selectable self = Helper.Get1DArrayElementBy2DIndexes(ref inventorySlots, width, i, j).Selectable;

				Navigation nav = new Navigation();

				//NOTE:	We only bother testing if slots are interactable to the right and down since the
				//		inventory fills up from the top-right corner
				if (self.IsInteractable())
				{
					nav.mode = Navigation.Mode.Explicit;

					///Horizontal navigation
					//Left should be the slot on left, unless this is the leftmost slot, then left is first menu button (if it exists)
					nav.selectOnLeft = i > 0 ? Helper.Get1DArrayElementBy2DIndexes(ref inventorySlots, width, i - 1, j).Selectable : (menuButtons.Length > 0 ? menuButtons[0] : null);

					//Right should be the slot on the right (clamped & only if interactable)
					Selectable slotRight = i < width - 1 ? Helper.Get1DArrayElementBy2DIndexes(ref inventorySlots, width, i + 1, j).Selectable : null;
					nav.selectOnRight = slotRight ? (slotRight.IsInteractable() ? slotRight : null) : null;

					///Vertical navigation
					//Slot above (clamped)
					nav.selectOnUp = j > 0 ? Helper.Get1DArrayElementBy2DIndexes(ref inventorySlots, width, i, j - 1).Selectable : null;

					//Slot below (clamped & only if interactable)
					Selectable slotBelow = j < height - 1 ? Helper.Get1DArrayElementBy2DIndexes(ref inventorySlots, width, i, j + 1).Selectable : null;
					nav.selectOnDown = slotBelow ? (slotBelow.IsInteractable() ? slotBelow : null) : null;

				}
				else
					nav.mode = Navigation.Mode.None;

				self.navigation = nav;
			}
		}
	}
}
