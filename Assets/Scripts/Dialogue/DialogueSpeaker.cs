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

    private void Start()
    {
        //Load json from text asset
        TextAsset asset = Resources.Load<TextAsset>(resourceFile);
        json = asset.text;

        //Deserialise json into DialogueGraph
        graph = JsonUtility.FromJson<DialogueGraph>(json);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When the player enters the range
        if (collision.tag == "Player")
        {
            DialogueBox.instance.ShowDialogue(graph, transform.position + (Vector3)boxOffset);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //When the player exits the range
        if (collision.tag == "Player")
        {
            DialogueBox.instance.gameObject.SetActive(false);
        }
    }
}
