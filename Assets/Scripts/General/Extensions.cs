using UnityEngine;
using System.Collections;

/// <summary>
/// Generic class containing useful extension methods
/// </summary>
public static class Extensions
{
	public static Bounds LerpTo(this Bounds self, Bounds target, float t)
	{
		Bounds bounds = new Bounds();

		bounds.min = Vector3.Lerp(self.min, target.min, t);
		bounds.max = Vector3.Lerp(self.max, target.max, t);

		return bounds;
	}
}
