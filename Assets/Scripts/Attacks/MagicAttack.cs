﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MagicAttack", menuName ="Data/Magic Attack")]
public class MagicAttack : ScriptableObject
{
    public string displayName = "Basic Attack";
    public Sprite icon;

    [Space()]
    public int manaCost = 10;
    public float cooldownTime = 1f;
    public int manaAmount = 100;

    [Space()]
    public GameObject castEffect;

    [Space()]
    public GameObject projectilePrefab;
}
