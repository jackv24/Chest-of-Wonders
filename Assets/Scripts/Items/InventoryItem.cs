﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[CreateAssetMenu(fileName ="NewItem", menuName ="Data/Item")]
public class InventoryItem : ScriptableObject
{
    [Serializable]
    public struct PlayerStatsEffect
    {
        [Range(-1.0f, 1.0f)]
        public float HealthPercentage;

        [Range(-1.0f, 1.0f)]
        public float ManaPercentage;
    }

    [SerializeField]
    private LocalizedString displayName;
    public string DisplayName { get { return displayName; } }

    [SerializeField]
    private LocalizedString description;
    public string Description { get { return description; } }

    [SerializeField]
    private Sprite inventoryIcon;
    public Sprite InventoryIcon { get { return inventoryIcon; } }

    [SerializeField]
    private int maxPocketAmount;
    public int MaxPocketAmount { get { return maxPocketAmount; } }

    [SerializeField]
    private PlayerStatsEffect statsEffect;
    public PlayerStatsEffect StatsEffect { get { return statsEffect; } }

    public int PocketAmount { get { return PlayerInventory.Instance.GetItemCount(this); } }

    public bool Use()
    {
        if (PocketAmount <= 0)
            return false;

        bool didUse = false;
        PlayerStats playerStats = GameManager.instance.player.GetComponent<PlayerStats>();

        int healthOffset = Mathf.RoundToInt(playerStats.MaxHealth * statsEffect.HealthPercentage);
        if (healthOffset != 0)
        {
            if (healthOffset > 0 && playerStats.AddHealth(healthOffset))
                didUse = true;
            else if (playerStats.RemoveHealth(-healthOffset))
                didUse = true;
        }

        return didUse;
    }
}
