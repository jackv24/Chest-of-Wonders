using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    public static ElementManager instance;

    //Public so that unity serialises it. Custom inspector overrides
    public float[] damageArray = new float[]
        {
            1.0f, 1.0f, 1.0f, 1.0f,
            0.75f, 1.0f, 0.75f, 0.5f,
            1.0f, 1.25f, 1.0f, 1.5f,
            1.25f, 1.5f, 0.75f, 1.0f
        };

    public enum Element
    {
        Basic = 0,
        Fire,
        Ice,
        Grass
    }

    void Awake()
    {
        instance = this;
    }

    public float GetDamageValue(int x, int y)
    {
        return damageArray[y * System.Enum.GetNames(typeof(Element)).Length + x];
    }

    public void SetDamageValue(int x, int y, float value)
    {
        damageArray[y * System.Enum.GetNames(typeof(Element)).Length + x] = value;
    }

    public static int CalculateDamage(int sourceDamage, Element sourceElement, Element targetElement)
    {
        float newDamage = sourceDamage * instance.GetDamageValue((int)targetElement, (int)sourceElement);

        return Mathf.RoundToInt(newDamage);
    }
}
