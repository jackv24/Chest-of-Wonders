using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MagicAttack", menuName ="Data/Magic Attack")]
public class MagicAttack : ScriptableObject
{
    public string displayName = "Basic Attack";

    public ElementManager.Element element;
    public int manaCost = 10;

	public enum Type
	{
		Projectile,
		Animation
	}
	public Type type;

    public GameObject castEffect;
    public GameObject projectilePrefab;

	public AnimationClip animation;
}
