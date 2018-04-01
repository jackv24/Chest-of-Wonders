using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem itemData;

    public float pickupRange = 1.0f;

    public LayerMask playerLayer;

    public GameObject pickupEffect;
    public SoundEffectBase.SoundEffect pickupSound;

    private SoundEffectBase soundEffects;

	public PersistentObject persistentObject;

    void Start()
    {
        soundEffects = GameManager.instance.GetComponent<SoundEffectBase>();

		bool pickedUp = false;
		persistentObject.GetID(gameObject);
		persistentObject.LoadState(ref pickedUp);

        if (pickedUp)
            gameObject.SetActive(false);
    }

    void Update()
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
					persistentObject.SaveState(true);

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
