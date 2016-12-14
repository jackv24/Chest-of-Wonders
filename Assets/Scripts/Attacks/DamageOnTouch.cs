﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Gameobjects with this tag will be damaged.")]
    public string damageTag = "Enemy";

    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;

    [Space()]
    [Tooltip("The effect to show when something is hit.")]
    public GameObject hitEffect;

    private List<GameObject> hitInSwing = new List<GameObject>();

    private void OnEnable()
    {
        hitInSwing.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == damageTag && !hitInSwing.Contains(collision.gameObject))
        {
            hitInSwing.Add(collision.gameObject);

            Vector3 centre = (GetComponent<Collider2D>().bounds.center + collision.bounds.center) / 2;

            //Show hit effect at centre of colliders (with object pooling)
            GameObject effect = ObjectPooler.GetPooledObject(hitEffect);
            effect.transform.position = centre;

            CharacterStats stats = collision.GetComponent<CharacterStats>();

            if(stats)
            {
                stats.RemoveHealth(amount);
            }
        }
    }
}
