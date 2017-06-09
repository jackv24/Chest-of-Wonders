using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthContainer : MonoBehaviour
{
    public int amount = 15;

    [Space()]
    public float pickupRadius = 0.5f;
    public Vector2 pickupOffset;

    [Space()]
    public LayerMask pickupLayer;

    [Space()]
    public GameObject pickupEffect;
    public SoundEffectBase.SoundEffect pickupSound;

    private SoundEffectBase soundEffects;

    void Start()
    {
        soundEffects = GameManager.instance.GetComponent<SoundEffectBase>();
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)pickupOffset, pickupRadius, pickupLayer);

        foreach (Collider2D col in colliders)
        {
            if (col.tag == "Player")
            {
                CharacterStats stats = col.GetComponent<CharacterStats>();

                if (stats)
                {
                    if (stats.AddHealth(amount))
                    {
                        if (pickupEffect)
                        {
                            GameObject obj = ObjectPooler.GetPooledObject(pickupEffect);
                            obj.transform.position = transform.position;
                        }

                        if (soundEffects)
                            soundEffects.PlaySound(pickupSound);

                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)pickupOffset, pickupRadius);
    }
}
