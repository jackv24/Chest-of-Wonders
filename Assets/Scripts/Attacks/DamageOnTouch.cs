using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : Damager
{
	[Tooltip("If the assigned CharacterStats is immune, it will prevent this damager from causing damage.")]
    public CharacterStats characterStats;

	private List<IDamageable> damageablesInside = new List<IDamageable>();

    private void OnDisable()
    {
		damageablesInside.Clear();
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		IDamageable damageable = collision.GetComponent<IDamageable>();
		if (damageable != null && !damageablesInside.Contains(damageable))
		{
			damageablesInside.Add(damageable);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		IDamageable damageable = collision.GetComponent<IDamageable>();
		if (damageable != null && damageablesInside.Contains(damageable))
		{
			damageablesInside.Remove(damageable);
		}
	}

	private void FixedUpdate()
    {
        //Character can not cause damage if it can not take damage
        if (characterStats && characterStats.damageImmunity)
            return;

		foreach(var damageable in damageablesInside)
		{
			TryCauseDamage(damageable);
		}
    }
}
