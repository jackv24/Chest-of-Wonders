using UnityEngine;
using System.Collections;
using I2.Loc;
using UnityEngine.UI;

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

	public static IEnumerator PlayWait(this Animator self, string stateName, float normalizedTime = 0)
	{
		self.Play(stateName, 0, normalizedTime);

		if (normalizedTime < 1)
		{
			yield return null;

			yield return new WaitForSeconds(self.GetCurrentAnimatorStateInfo(0).length * (1 - normalizedTime));
		}
	}

	public static bool CanSelect(this Selectable self)
	{
		return self.IsInteractable() && self.gameObject.activeInHierarchy;
	}

	public static void SetRotationZ(this Transform self, float rotationZ)
	{
		Vector3 localRotation = self.localEulerAngles;
		localRotation.z = rotationZ;
		self.localEulerAngles = localRotation;
	}
}
