using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStats : CharacterStats
{
    public Action<int, int> OnHealthUpdated;

	private void Start()
	{
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				currentHealth = data.CurrentHealth;
				maxHealth = data.MaxHealth;

                HealthUpdated();
            };

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
				if (hardSave)
					currentHealth = maxHealth;

				data.CurrentHealth = currentHealth;
				data.MaxHealth = maxHealth;
			};
		}
	}

    protected override Vector2 GetKnockBackVelocity(DamageProperties damageProperties)
	{
		Vector2 direction = new Vector2(Mathf.Sign(damageProperties.direction.x), 1.0f).normalized;

		float speed = knockbackDistance / knockbackTime;

		return direction * speed;
	}

    protected override void HealthUpdated()
    {
        OnHealthUpdated?.Invoke(currentHealth, maxHealth);
    }
}
