using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public static DialogueEvents instance;

    private void Awake()
    {
        instance = this;
    }

    public void HandleEvents(DialogueGraph.DialogueGraphNode.Events events, GameObject speaker)
    {
        //Only bother checking events if any are used
        if(events.useEvent)
        {
            //Save game (hard)
            if (events.saveGame)
                SaveManager.instance.SaveGame(true);

            //Start coroutine to move speaker
            if (events.moveX != 0)
                StartCoroutine(MoveCharacter(events.moveX, speaker));
        }
    }

    IEnumerator MoveCharacter(float distance, GameObject character)
    {
        float targetPos = character.transform.position.x + distance;
        float sign = Mathf.Sign(distance);

        CharacterMove move = character.GetComponent<CharacterMove>();

        if (move)
        {
            //Keep moving in direction until reached target spot
            while ((sign > 0 && character.transform.position.x < targetPos) || (sign < 0 && character.transform.position.x > targetPos))
            {
                move.Move(sign);

                yield return new WaitForEndOfFrame();
            }

            //Stop movement
            move.Move(0);
        }
        else
            Debug.LogWarning("No CharacterMove attached to " + character.name);
    }
}
