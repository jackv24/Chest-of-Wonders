using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    public int targetLevelIndex = 2;

    public Vector2 scenePosition;

    void OnTriggerEnter2D(Collider2D other)
    {
        //If a player enters this doorway, load the target scene with the player at a position
        if(other.tag == "Player")
            GameManager.instance.LoadLevel(targetLevelIndex, scenePosition);
    }
}
