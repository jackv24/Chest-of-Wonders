using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnable : Damager
{
    private List<GameObject> hitInSwing = new List<GameObject>();

	private void OnDisable()
    {
        hitInSwing.Clear();
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!enabled)
			return;

		GameObject obj = collider.gameObject;

		if (!hitInSwing.Contains(obj))
		{
			hitInSwing.Add(obj);

			TryCauseDamage(obj.GetComponent<IDamageable>());
		}
	}
}
