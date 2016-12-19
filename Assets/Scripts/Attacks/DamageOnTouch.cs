using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Gameobjects with this tag will be damaged.")]
    public string damageTag = "Enemy";

    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;

    [Header("Effects")]
    [Tooltip("The effect to show when something is hit.")]
    public GameObject hitEffect;

    [Space()]
    public float screenShakeAmount = 0.5f;
    public float knockBackAmount = 10f;
    public Vector2 knockBackCentreOffset = -Vector2.up;

    private List<GameObject> hitInSwing = new List<GameObject>();

    private void OnEnable()
    {
        hitInSwing.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == damageTag && !hitInSwing.Contains(collision.gameObject))
        {
            //Keep track of what has already been
            hitInSwing.Add(collision.gameObject);

            //Calculate centre point between colliders to show hit effect
            Vector3 centre = (GetComponent<Collider2D>().bounds.center + collision.bounds.center) / 2;

            //Show hit effect at centre of colliders (with object pooling)
            GameObject effect = ObjectPooler.GetPooledObject(hitEffect);
            effect.transform.position = centre;

            //Get character references
            CharacterStats stats = collision.GetComponent<CharacterStats>();
            CharacterMove move = collision.GetComponent<CharacterMove>();

            //Remove health
            if(stats)
                stats.RemoveHealth(amount);

            //Knockback if amount is more than 0
            if (move && knockBackAmount > 0)
                move.Knockback((Vector2)transform.position + knockBackCentreOffset, knockBackAmount);

            //Offset randomly (screen shake effect)
            Vector2 camOffset = new Vector2(Random.Range(-1f, 1f) * screenShakeAmount, Random.Range(-1f, 1f) * screenShakeAmount);
            Camera.main.transform.position += (Vector3)camOffset;
        }
    }
}
