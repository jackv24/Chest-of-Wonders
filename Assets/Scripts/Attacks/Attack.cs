using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class Attack : ScriptableObject
{
    //The name to display in-game for this attack
    public string displayName;

    public enum Type
    {
        Melee,
        Magic
    }
    //The type of attack this is
    public Type type;

    //How much mana this attack costs to use
    public int manaCost = 0;

    public void Use()
    {
        Debug.Log("Used \"" + displayName + "\" attack. Type: " + type);
    }
}
