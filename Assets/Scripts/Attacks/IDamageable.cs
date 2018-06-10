using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
	Regular,
	Projectile
}

public struct DamageProperties
{
	public int amount;
	public ElementManager.Element sourceElement;
	public DamageType type;
	public Vector2 direction;
}

public interface IDamageable
{
	/// <summary>
	/// Causes this damageable to take damage.
	/// </summary>
	/// <param name="amount">The amount of damage to take.</param>
	/// <param name="element">The source element of the damage.</param>
	/// <returns>Returns true if damage was taken, would return false if target is invincible, etc.</returns>
	bool TakeDamage(DamageProperties damageProperties);
}
