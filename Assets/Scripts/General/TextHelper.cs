using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TextHelper
{
    private static readonly Dictionary<char, string> styleMapping = new Dictionary<char, string>
    {
        { '*', $"<color=#{GlobalTextSettings.EmphasisedTextColor.ToHTML()}>{{0}}</color>" }
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
                mainBuilder.Append(string.Format(styleMapping[key], subBuilder.ToString()));

                // Continue parsing text AFTER the previous style key
                i = continueIndex;
            }
        }

        return mainBuilder.ToString();
    }
}
