using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    //Where to load the dialog json file
    public string resourceFile = "Dialogue/DialogueFile";

    [Space()]
    //How far offset from the gameobject should it be (in world space)
    public Vector2 boxOffset;

    //string to store the json text
    private string json;
    //DialogueGraph to store the deserialised json
    private DialogueGraph graph;

    private PlayerActions playerActions;
    private bool inRange = false;

    private void Start()
    {
        //Load json from text asset
        TextAsset asset = Resources.Load<TextAsset>(resourceFile);
        json = asset.text;

        //Deserialise json into DialogueGraph
        graph = JsonUtility.FromJson<DialogueGraph>(json);

        playerActions = new PlayerActions();
    }

    private void Update()
    {
        //If interact buttons is pressed in range...
        if (inRange && playerActions.Interact.WasPressed)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When the player enters the range
        if (collision.tag == "Player")
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //When the player exits the range
        if (collision.tag == "Player")
        {
            inRange = false;
        }
    }
}
