using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem itemData;

    public float pickupRange = 1.0f;

    public LayerMask playerLayer;

    public GameObject pickupEffect;

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
