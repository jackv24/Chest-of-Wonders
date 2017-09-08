using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    public static ElementManager instance;

	public static int ElementCount { get { return System.Enum.GetNames(typeof(Element)).Length; } }

	public MagicAttack fireAttack;
	public MagicAttack grassAttack;
	public MagicAttack iceAttack;
	public MagicAttack windAttack;

	public MagicAttack[] attackArray = new MagicAttack[ElementCount * ElementCount];

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
        Fire = 0,
        Grass,
        Ice,
        Wind
    }

    void Awake()
    {
        instance = this;
    }

	public MagicAttack GetAttack(int x, int y, bool skipDefault = false)
	{
		MagicAttack attack = attackArray[y * System.Enum.GetNames(typeof(Element)).Length + x];

		//If attack is null and default is desired, default to base element
		if(!skipDefault && !attack)
		{
			//Column major, so y is for rows
			switch((Element)y)
			{
				case Element.Fire:
					attack = fireAttack;
					break;
				case Element.Grass:
					attack = grassAttack;
					break;
				case Element.Ice:
					attack = iceAttack;
					break;
				case Element.Wind:
					attack = windAttack;
					break;
			}
		}

		return attack;
	}

	public void SetAttack(int x, int y, MagicAttack value)
	{
		attackArray[y * System.Enum.GetNames(typeof(Element)).Length + x] = value;
	}

	public Effectiveness GetEffectiveness(int x, int y)
    {
        return damageArray[y * System.Enum.GetNames(typeof(Element)).Length + x];
    }

    public void SetEffectiveness(int x, int y, Effectiveness value)
    {
        damageArray[y * System.Enum.GetNames(typeof(Element)).Length + x] = value;
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

	public static MagicAttack GetAttack(Element baseElement, Element mixElement)
	{
		if (instance)
		{
			//Cast enums to int for accessing attack matrix
			return instance.GetAttack((int)mixElement, (int)baseElement);
		}
		else
		{
			Debug.LogError("There is no ElementManager present!");

			return null;
		}
	}
}
