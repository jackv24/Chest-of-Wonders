using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    private static ElementManager instance;

	public static int ElementCount { get { return System.Enum.GetNames(typeof(Element)).Length; } }

	public float normalMultiplier = 1.0f;
	public float ineffectiveMultiplier = 0.5f;
	public float effectiveMultiplier = 1.25f;

	public enum Effectiveness
	{
		Normal,
		Ineffective,
		Effective,
	}

	//Public so that unity serialises it. Custom inspector overrides
	public Effectiveness[] damageArray = new Effectiveness[ElementCount * ElementCount];

    public enum Element
    {
		None = 0,
        Fire,
        Grass,
        Ice,
        Wind
    }

    void Awake()
    {
        instance = this;
    }

	public Effectiveness GetEffectiveness(int x, int y)
    {
        return damageArray[Get1DArrayElementFrom2DIndexes(x, y)];
    }

    public void SetEffectiveness(int x, int y, Effectiveness value)
    {
        damageArray[Get1DArrayElementFrom2DIndexes(x, y)] = value;
    }

    private int Get1DArrayElementFrom2DIndexes(int x, int y)
    {
        return y * System.Enum.GetNames(typeof(Element)).Length + x;
    }

    public static int CalculateDamage(int sourceDamage, Element sourceElement, Element targetElement)
    {
		float newDamage = sourceDamage;

		if (instance)
		{
			Effectiveness effectiveNess = instance.GetEffectiveness((int)targetElement, (int)sourceElement);

			switch (effectiveNess)
			{
				case Effectiveness.Normal:
					newDamage *= instance.normalMultiplier;
					break;
				case Effectiveness.Ineffective:
					newDamage *= instance.ineffectiveMultiplier;
					break;
				case Effectiveness.Effective:
					newDamage *= instance.effectiveMultiplier;
					break;
			}
		}

        return Mathf.RoundToInt(newDamage);
    }

    public static Effectiveness GetEffectiveness(Element sourceElement, Element targetElement)
    {
        if (!instance)
            return Effectiveness.Normal;

        return instance.GetEffectiveness((int)targetElement, (int)sourceElement);
    }
}
