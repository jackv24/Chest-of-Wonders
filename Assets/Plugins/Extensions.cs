using UnityEngine;
using System.Collections;
using I2.Loc;

/// <summary>
/// Generic class containing useful extension methods
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Attempts to get the translation of this string. If no translation is found the
	/// original string will be returned with some warning characters to signify that it has not been translated
	/// </summary>
	public static string TryGetTranslation(this string self)
	{
		string translation;

		//Try and get translation. If this text was not a translation key then show as warning
		if (!LocalizationManager.TryGetTranslation(self, out translation))
			translation = $"#{self}#";

		return translation;
	}
}
