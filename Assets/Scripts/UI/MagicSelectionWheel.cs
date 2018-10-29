using UnityEngine;
using InControl;

public class MagicSelectionWheel : ButtonSelectionWheel
{
    [SerializeField, ArrayForEnum(typeof(Direction))]
    private ElementManager.Element[] directionMappings;

    private PlayerAttack playerAttack;

    protected override PlayerAction HoldButton
    {
        get { return Actions.MagicSelection; }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        ArrayForEnumAttribute.EnsureArraySize(ref directionMappings, typeof(Direction));
    }

    protected override int GetSelectedDirection()
    {
        for (int i = 0; i < directionMappings.Length; i++)
        {
            if (directionMappings[i] == playerAttack.selectedElement)
                return i;
        }

        return -1;
    }

    protected override void DirectionConfirmed(Direction direction)
    {
        playerAttack.SetSelectedMagic(directionMappings[(int)direction]);
    }

    protected override void Start()
    {
        base.Start();

        playerAttack = GameManager.instance.player.GetComponent<PlayerAttack>();
    }
}
