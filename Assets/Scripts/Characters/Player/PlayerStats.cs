using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStats : CharacterStats
{
    public Action<int, int> OnHealthUpdated;
    public Action<int, int> OnManaUpdated;

    [Header("Player Stats")]
    [SerializeField]
    private int currentMana = 100;
    public int CurrentMana { get { return currentMana; } }

    [SerializeField]
    private int maxMana = 100;
    public int MaxMana { get { return maxMana; } }

    [SerializeField]
    private float manaRegenDelay = 1.0f;
    private float manaRegenStartTime;

    [SerializeField]
    private float manaRegenSpeed = 10.0f;
    private float manaRegenRemainder;

	private void Start()
	{
		if(SaveManager.instance)
		{
			SaveManager.instance.OnDataLoaded += (SaveData data) =>
			{
				currentHealth = data.CurrentHealth;
				maxHealth = data.MaxHealth;

                maxMana = data.MaxMana;
                currentMana = maxMana;

                HealthUpdated();
            };

			SaveManager.instance.OnDataSaving += (SaveData data, bool hardSave) =>
			{
                if (hardSave)
                {
                    currentHealth = maxHealth;
                }

				data.CurrentHealth = currentHealth;
				data.MaxHealth = maxHealth;

                data.MaxMana = maxMana;
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

    public bool AddMana(int amount)
    {
        if (currentMana >= maxMana || amount <= 0)
            return false;

        currentMana += amount;
        if (currentMana > maxMana)
            currentMana = maxMana;

        OnManaUpdated?.Invoke(currentMana, maxMana);

        return true;
    }

    public bool RemoveMana(int amount)
    {
        if (currentMana <= 0 || amount <= 0)
            return false;

        currentMana -= amount;
        if (currentMana < 0)
            currentMana = 0;

        OnManaUpdated?.Invoke(currentMana, maxMana);

        manaRegenStartTime = Time.time + manaRegenDelay;

        return true;
    }

    private void Update()
    {
        // Regenerate mana
        if (currentMana < maxMana && Time.time >= manaRegenStartTime)
        {
            // Calculate amount to regen this frame (and store remainder if not whole number so regen is not frame-dependent)
            float regenAmount = (manaRegenSpeed * Time.deltaTime) + manaRegenRemainder;
            manaRegenRemainder = regenAmount % 1;

            AddMana((int)regenAmount);
        }
    }
}
