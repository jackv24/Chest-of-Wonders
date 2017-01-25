using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public delegate void NormalEvent();

    public event NormalEvent OnCycleMagic;

    private CharacterAnimator characterAnimator;

    public MagicAttack[] magicAttacks;
    [HideInInspector]
    public int selectedMagic = 0;

    [HideInInspector]
    public int[] manaAmounts;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    void Start()
    {
        InitialiseMana();
    }

    public void UseMelee()
    {
        //Play melee animation
        if (characterAnimator)
            characterAnimator.MeleeAttack();
    }

    public void UseMagic()
    {
        if (selectedMagic >= 0 && selectedMagic < magicAttacks.Length)
        {
            if (manaAmounts[selectedMagic] >= magicAttacks[selectedMagic].manaCost)
            {
                manaAmounts[selectedMagic] -= magicAttacks[selectedMagic].manaCost;

                //TODO: Cast magic
            }
        }
        else
            Debug.LogWarning("Selected Attack is out of bounds! Attack: " + selectedMagic);

        //Play attack animation
        if (characterAnimator)
            characterAnimator.MagicAttack();
    }

    public void CycleMagic(int direction)
    {
        //Add direction
        selectedMagic += direction;

        //If selected attack is out of bounds, wrap around
        if (selectedMagic >= magicAttacks.Length)
            selectedMagic = 0;
        else if (selectedMagic < 0)
            selectedMagic = magicAttacks.Length - 1;

        if (OnCycleMagic != null)
            OnCycleMagic();
    }

    void InitialiseMana()
    {
        manaAmounts = new int[magicAttacks.Length];

        for(int i = 0; i < manaAmounts.Length; i++)
        {
            manaAmounts[i] = magicAttacks[i].manaAmount;
        }
    }
}
