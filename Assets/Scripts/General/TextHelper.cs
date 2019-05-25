using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public static class TextHelper
{
    public const char SHORTCUT_EMPHASIS = '*';
    public const char SHORTCUT_BUTTON = '%';
    public const char SHORTCUT_SMALL = '_';

    private static readonly Dictionary<char, Func<string, string>> styleMapping = new Dictionary<char, Func<string, string>>
    {
        // Emphasis
        { SHORTCUT_EMPHASIS, text => $"<color=#{GlobalTextSettings.EmphasisedTextColor.ToHTML()}>{text}</color>" },

        // Button Sprites
        { SHORTCUT_BUTTON, text => GetButtonText(text, 32.0f) }, // 32.0f just so happens to be correct, may need to change if we change dialogue font

        // Small text
        { SHORTCUT_SMALL, text => ApplyTextStyle(text, GlobalTextSettings.WhisperStyle) },
    };

    /// <summary>
	/// Parses game text, stripping whitespace and handling any tags.
	/// </summary>
	/// <param name="original">The text to parse.</param>
	/// <returns>An array of string split at every "<pg>" tag.</returns>
	public static string[] ParseGameText(string original)
    {
        //Split text into pages first before working on each page
        string[] pages = original.Split(new string[] { "<pg>" }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < pages.Length; i++)
        {
            string[] parts = pages[i].Split(new string[] { "<br>" }, System.StringSplitOptions.RemoveEmptyEntries);

            //Parse each string part then re-join them
            StringBuilder builder = new StringBuilder(parts.Length * 2);
            for (int j = 0; j < parts.Length; j++)
            {
                builder.Append(parts[j].Trim());

                //Append new line to all except last
                if (j < parts.Length - 1)
                    builder.Append('\n');
            }

            pages[i] = ParseTextStyling(builder.ToString());
        }

        return pages;
    }

    private static string ParseTextStyling(string original)
    {
        int length = original.Length;
        StringBuilder mainBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            char key = original[i];
            if (!styleMapping.ContainsKey(key))
                mainBuilder.Append(key);

            // Style key has been found, extract text until next one
            else if (i < length - 1)
            {
                int startIndex = i + 1;
                int continueIndex = length;
                StringBuilder subBuilder = new StringBuilder(continueIndex - startIndex);

                for (int j = startIndex; j < length; j++)
                {
                    if (original[j].Equals(key))
                    {
                        continueIndex = j;
                        break;
                    }
                    else
                        subBuilder.Append(original[j]);
                }

                // Format text between style keys and add to builder
                mainBuilder.Append(styleMapping[key](subBuilder.ToString()));

                // Continue parsing text AFTER the previous style key
                i = continueIndex;
            }
        }

        return mainBuilder.ToString();
    }

    private static string ApplyTextStyle(string text, TextStyle style)
    {
        return $"<font=\"{style.Font.name}\"><size={style.Size}>{text}</size></font>";
    }

    public static string GetButtonText(string original, float spriteFontSize)
    {
        var playerActions = ControlManager.GetPlayerActions();
        string buttonName = playerActions.GetBoundButtonName((PlayerActions.ButtonActionType)Enum.Parse(typeof(PlayerActions.ButtonActionType), original));

        if (playerActions.IsUsingKeyboard)
        {
            return $"<color=#{GlobalTextSettings.EmphasisedTextColor.ToHTML()}>{buttonName}</color>";
        }
        else
        {
            return string.Format(
                "<size={2}><sprite=\"{0} Buttons\" name=\"{1}\"></size>",
                ControlManager.GetButtonDisplayType().ToString(),
                buttonName,
                spriteFontSize
                );
        }
    }
}

[Serializable]
public struct TextStyle
{
    public TMP_FontAsset Font;
    public int Size;
}
