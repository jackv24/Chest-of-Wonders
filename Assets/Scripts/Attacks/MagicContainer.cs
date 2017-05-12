using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicContainer : MonoBehaviour
{
    public MagicAttack attack;

    [Space()]
    public float pickupRange = 2.0f;
    public float buttonHoldTime = 1.0f;
    private float buttonHeldTime = 0;

    private PlayerActions playerActions;

    void Start()
    {
        playerActions = new PlayerActions();
    }

    void Update()
    {
        //Get all colliders in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        foreach(Collider2D col in cols)
        {
            //If player is in range
            if (col.tag == "Player")
            {
                if (playerActions.AbsorbMagic.IsPressed)
                {
                    //Count time button is held
                    buttonHeldTime += Time.deltaTime;

                    //If button is held for long enough, absorb attack
                    if (buttonHeldTime >= buttonHoldTime)
                        Absorb(col.gameObject);
                }
                else
                    buttonHeldTime = 0;
            }
        }
    }

    void Absorb(GameObject player)
    {
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        if(playerAttack)
        {
            //If slot 2 is empty move current attack into that slot to fit the new one
            if (playerAttack.magicSlot2.attack == null)
                playerAttack.SwitchMagic();

            //Overwrite attack in slot 1
            playerAttack.magicSlot1.attack = attack;
            playerAttack.magicSlot1.currentMana = attack.manaAmount;

            //Make sure UI is updated
            playerAttack.UpdateMagic();

            //Destroy container
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
