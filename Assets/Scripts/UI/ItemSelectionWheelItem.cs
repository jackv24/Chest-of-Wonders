using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSelectionWheelItem : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private TextMeshProUGUI amountText;

    public void SetDisplay(InventoryItem item)
    {
        if (!item)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (icon)
        {
            icon.sprite = item.InventoryIcon;
            icon.SetNativeSize();
        }

        if (amountText)
        {
            amountText.text = item.PocketAmount.ClampMin(0).ToString();
        }
    }
}
