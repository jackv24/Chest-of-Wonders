using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnable : MonoBehaviour
{
    public LayerMask damageLayer;

    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;
    public float multiplier = 1.0f;

    [Header("Effects")]
    [Tooltip("The effect to show when something is hit.")]
    public GameObject hitEffect;

    [Space()]
    public float screenShakeAmount = 0.5f;
    public float knockBackAmount = 10f;
    public Vector2 knockBackCentreOffset = -Vector2.up;

    private List<GameObject> hitInSwing = new List<GameObject>();

    private Collider2D col;
    private Rect box;

    private void OnEnable()
    {
        hitInSwing.Clear();

        col = GetComponent<Collider2D>();

        box = new Rect(
            col.bounds.min.x,
            col.bounds.min.y,
            col.bounds.size.x,
            col.bounds.size.y
            );
    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(box.center, box.size, 0, damageLayer);

        foreach (Collider2D collider in colliders)
        {
            if (!hitInSwing.Contains(collider.gameObject))
            {
                //Keep track of what has already been hit (in case gameobject has multiple colliders)
                hitInSwing.Add(collider.gameObject);

                //Calculate centre point between colliders to show hit effect
                Vector3 centre = (box.center + (Vector2)collider.bounds.center) / 2;

                //Show hit effect at centre of colliders (with object pooling)
                GameObject effect = ObjectPooler.GetPooledObject(hitEffect);
                effect.transform.position = centre;

                //Get character references
                CharacterStats stats = collider.GetComponent<CharacterStats>();
                CharacterMove move = collider.GetComponent<CharacterMove>();

                //Remove health
                if (stats)
                    stats.RemoveHealth(Mathf.RoundToInt((float)amount * multiplier));

                //Knockback if amount is more than 0
                if (move && knockBackAmount > 0)
                    move.Knockback((Vector2)transform.position + knockBackCentreOffset, knockBackAmount);

                //Offset randomly (screen shake effect)
                Vector2 camOffset = new Vector2(Random.Range(-1f, 1f) * screenShakeAmount, Random.Range(-1f, 1f) * screenShakeAmount);
                Camera.main.transform.position += (Vector3)camOffset;
            }
        }
    }
}
