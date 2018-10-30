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

    protected override bool DirectionConfirmed(Direction direction)
    {
        if (!IsEnabled(direction))
            return false;

        playerAttack.SetSelectedMagic(directionMappings[(int)direction]);
        return true;
    }

    protected override void Start()
    {
        base.Start();

        playerAttack = GameManager.instance.player.GetComponent<PlayerAttack>();
    }

    protected override bool IsEnabled(Direction direction)
    {
        switch (directionMappings[(int)direction])
        {
            case ElementManager.Element.Fire:
                return playerAttack.hasFireMagic;

            case ElementManager.Element.Grass:
                return playerAttack.hasGrassMagic;

            case ElementManager.Element.Ice:
                return playerAttack.hasIceMagic;

            case ElementManager.Element.Wind:
                return playerAttack.hasWindMagic;

            default:
                return false;
        }
    }
}
