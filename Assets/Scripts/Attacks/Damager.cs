using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damager : MonoBehaviour
{
	[Tooltip("How much damage to deal to everything this hits.")]
	public int amount = 10;
	public ElementManager.Element element;

	public bool useFacingDirection = true;

	protected bool TryCauseDamage(IDamageable damageable)
	{
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

				return true;
			}
		}

		return false;
	}
}
