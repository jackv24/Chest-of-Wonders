using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    //Where to load the dialog json file
    public TextAsset dialogueFile;

    [Space()]
    //How far offset from the gameobject should it be (in world space)
    public Vector2 boxOffset;

    //string to store the json text
    private string json;
    //DialogueGraph to store the deserialised json
    private DialogueGraph graph;

    private PlayerActions playerActions;
    private bool inRange = false;

    public float range = 2f;
    private GameObject player;

    private void Start()
    {
        //Load json from text asset
        if (dialogueFile)
        {
            json = dialogueFile.text;

            //Deserialise json into DialogueGraph
            graph = JsonUtility.FromJson<DialogueGraph>(json);
        }

        player = GameObject.FindWithTag("Player");

        playerActions = new PlayerActions();
    }

    private void Update()
    {
        if (player)
        {
            if (Vector2.Distance(player.transform.position, transform.position) <= range)
                inRange = true;
            else
                inRange = false;
        }

        //If interact buttons is pressed in range...
        if (inRange && playerActions.Interact.WasPressed && graph != null)
        {
            //...and the player "can move" (can perform actions outside of UI)
            if (GameManager.instance.canMove)
            {
                //Stop them moving and open dialogue
                GameManager.instance.canMove = false;
                DialogueBox.instance.ShowDialogue(graph, transform.position + (Vector3)boxOffset);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
