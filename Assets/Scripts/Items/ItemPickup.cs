using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Tooltip("MUST BE UNIQUE")]
    public int uniqueID = 0;

    public InventoryItem itemData;

    public float pickupRange = 1.0f;

    public LayerMask playerLayer;

    public GameObject pickupEffect;
    public SoundEffectBase.SoundEffect pickupSound;

    private SoundEffectBase soundEffects;

    void Start()
    {
        soundEffects = GameManager.instance.GetComponent<SoundEffectBase>();

        //Check if this item has already been picked up
        if (SaveManager.instance.PickedUpItem(uniqueID))
            gameObject.SetActive(false);
    }

    //Only run on fixedupdate since this doesn't need to be run every frame
    void FixedUpdate()
    {
        if (itemData)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, pickupRange, playerLayer);

            foreach (Collider2D col in cols)
            {
                PlayerInventory inventory = col.GetComponent<PlayerInventory>();

                //If collider in range has inventory
                if (inventory)
                {
                    //Add to inventory
                    inventory.AddItem(itemData);

                    if (pickupEffect)
                    {
                        GameObject effect = ObjectPooler.GetPooledObject(pickupEffect);
                        effect.transform.position = transform.position;
                    }

                    if (soundEffects)
                        soundEffects.PlaySound(pickupSound);

                    //Record that this item has been picked up
                    SaveManager.instance.SetPickedUpItem(uniqueID);

                    //Remove item from world
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
