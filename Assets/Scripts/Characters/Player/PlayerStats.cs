using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
	private void Start()
	{
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				currentHealth = data.currentHealth;
				maxHealth = data.maxHealth;
			};

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				if (hardSave)
					currentHealth = maxHealth;

				data.currentHealth = currentHealth;
				data.maxHealth = maxHealth;
			};
		}
	}

	protected override Vector2 GetKnockBackVelocity(DamageProperties damageProperties)
	{
		float speed = knockbackDistance / knockbackTime;

		return damageProperties.direction.normalized * speed;
	}
}
