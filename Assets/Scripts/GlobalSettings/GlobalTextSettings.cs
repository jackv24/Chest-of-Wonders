﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Data/GlobalSettings/Text", fileName = FILENAME)]
public class GlobalTextSettings : ScriptableObject
{
    private const string FILENAME = "Text Settings";

    [SerializeField]
    private Color emphasisedTextColor;
    public static Color EmphasisedTextColor { get { return Get().emphasisedTextColor; } }

    [SerializeField]
    private TextStyle whisperStyle;
    public static TextStyle WhisperStyle { get { return Get().whisperStyle; } }

    private static GlobalTextSettings instance;

    private static GlobalTextSettings Get()
    {
        if (instance != null)
            return instance;

        instance = Resources.Load($"GlobalSettings/{FILENAME}") as GlobalTextSettings;

        return instance;
    }
}
