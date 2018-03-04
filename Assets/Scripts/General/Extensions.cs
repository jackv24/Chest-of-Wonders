using UnityEngine;
using System.Collections;

public static class Extensions
{
	public static Rect LerpTo(this Rect rect, Rect target, float t)
	{
		return new Rect(Vector2.Lerp(rect.position, target.position, t), Vector2.Lerp(rect.size, target.size, t));
	}
}
