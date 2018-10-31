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
			translation = $"{self}";

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

	public static void SetLocalPositionX(this Transform self, float value)
	{
		Vector3 localPosition = self.localPosition;
		localPosition.x = value;
		self.localPosition = localPosition;
	}

	public static void SetLocalPositionY(this Transform self, float value)
	{
		Vector3 localPosition = self.localPosition;
		localPosition.y = value;
		self.localPosition = localPosition;
	}

	public static void SetLocalPositionZ(this Transform self, float value)
	{
		Vector3 localPosition = self.localPosition;
		localPosition.z = value;
		self.localPosition = localPosition;
	}

	public static void SetPosition2D(this Transform self, Vector2 position)
	{
		Vector3 pos = self.position;
		pos.x = position.x;
		pos.y = position.y;

		self.position = pos;
	}

    public static int ClampMin(this int original, int minValue)
    {
        if (original < minValue)
            original = minValue;

        return minValue;
    }

    public static Vector2 Where(this Vector2 original, float? x = null, float? y = null)
    {
        return new Vector2(x ?? original.x, y ?? original.y);
    }

    public static Vector3 Where(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }

    public static Color Where(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
    }
}
