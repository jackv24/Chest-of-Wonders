using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementHelper
{
    public enum Element
    {
        Basic = 0,
        Fire,
        Ice,
        Grass
    }

    public static int CalculateDamage(int sourceDamage, Element sourceElement, Element targetElement)
    {
        float[,] damageMatrix = new float[,]
        {
            { 1.0f, 1.0f, 1.0f, 1.0f },
            { 0.75f, 1.0f, 0.75f, 0.5f },
            { 1.0f, 1.25f, 1.0f, 1.5f },
            { 1.25f, 1.5f, 0.75f, 1.0f }
        };

        float newDamage = sourceDamage * damageMatrix[(int)targetElement, (int)sourceElement];

        return Mathf.RoundToInt(newDamage);
    }
}
