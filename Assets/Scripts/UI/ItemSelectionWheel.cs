using UnityEngine;
using InControl;

public class ItemSelectionWheel : ButtonSelectionWheel
{
    [SerializeField, ArrayForEnum(typeof(Direction))]
    private ItemSelectionWheelItem[] itemDisplays;

    // TEMPORARY for testing
    [SerializeField, ArrayForEnum(typeof(Direction))]
    private InventoryItem[] testItems;

    protected override void OnValidate()
    {
        base.OnValidate();

        ArrayForEnumAttribute.EnsureArraySize(ref itemDisplays, typeof(Direction));
        ArrayForEnumAttribute.EnsureArraySize(ref testItems, typeof(Direction));
    }

    protected override PlayerAction HoldButton
    {
        get { return Actions.ItemSelection; }
    }

    protected override bool DirectionConfirmed(Direction direction)
    {
        var item = testItems[(int)direction];
        return item?.Use() ?? false;
    }

    protected override void OnOpen()
    {
        // Arrays should be same size, but just in case
        int size = Mathf.Min(itemDisplays.Length, testItems.Length);
        for (int i = 0; i < size; i++)
        {
            var item = testItems[i];
            itemDisplays[i].SetDisplay(item);
        }
    }

    protected override bool IsEnabled(Direction direction)
    {
        return testItems[(int)direction]?.PocketAmount > 0;
    }
}
