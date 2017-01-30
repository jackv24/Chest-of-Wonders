using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private CharacterAnimator characterAnimator;

    [System.Serializable]
    public class MagicSlot
    {
        public MagicAttack attack;
        public int currentMana;
    }

    public MagicSlot magicSlot1;
    public MagicSlot magicSlot2;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    void Start()
    {
        //If there is an attack in the second slot, but not the first slot
        if (!magicSlot1.attack && magicSlot2.attack)
        {
            //Move second magic attack to first slot
            magicSlot1.attack = magicSlot2.attack;
            magicSlot2.attack = null;
        }

        //Set starting mana
        if (magicSlot1.attack)
            magicSlot1.currentMana = magicSlot1.attack.manaAmount;
        if (magicSlot2.attack)
            magicSlot2.currentMana = magicSlot2.attack.manaAmount;
    }

    public void UseMelee()
    {
        //Play melee animation
        if (characterAnimator)
            characterAnimator.MeleeAttack();
    }

    //magic use functions to prevent index mismatch issues
    public void UseMagic1(Vector2 direction)
    {
        UseMagic(1, direction);
    }
    public void UseMagic2(Vector2 direction)
    {
        UseMagic(2, direction);
    }

    //Function to use magic, wrapped by other magic use functions
    void UseMagic(int number, Vector2 direction)
    {
        //The magic slot to use
        MagicSlot slot;

        //Choose correct magic slot
        switch(number)
        {
            case 1:
                slot = magicSlot1;
                break;
            case 2:
                slot = magicSlot2;
                break;
            default:
                slot = null;
                break;
        }

        //If magic slot was chosen correctly, and there is an attack in the slot
        if(slot != null && slot.attack)
        {
            //If there is enough mana to use this attack
            if (slot.currentMana >= slot.attack.manaCost)
            {
                //Subtract required mana
                slot.currentMana -= slot.attack.manaCost;

                //TODO: Cast attack
                Debug.Log(string.Format("Used Attack {0}: {1}", number, slot.attack.displayName));
            }
        }

        if(characterAnimator)
        {
            characterAnimator.animator.SetFloat("vertical", direction.y);
            characterAnimator.animator.SetTrigger("magic");
        }
    }
}
