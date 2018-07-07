using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnable : MonoBehaviour
{
    public LayerMask damageLayer;

    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;
	public ElementManager.Element element;

	public bool useFacingDirection = true;

    private List<GameObject> hitInSwing = new List<GameObject>();

	private void OnEnable()
    {
        hitInSwing.Clear();
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!hitInSwing.Contains(collider.gameObject))
		{
			//Keep track of what has already been hit (in case gameobject has multiple colliders)
			hitInSwing.Add(collider.gameObject);

			//Get character references
			IDamageable damageable = collider.GetComponentInParent<IDamageable>();

			//Remove health
			if (damageable != null)
			{
				if (damageable.TakeDamage(new DamageProperties
				{
					amount = amount,
					sourceElement = element,
					type = DamageType.Regular,
					direction = useFacingDirection ? (transform.lossyScale.x < 0 ? Vector2.left : Vector2.right) : Vector2.zero
				}))
				{
					//TODO: Hit effects
				}
			}
		}
	}
}
