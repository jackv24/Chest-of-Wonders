using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    //Where to load the dialog json file
    public TextAsset dialogueFile;
    public Color windowColor = Color.grey;

    [Space()]
    //How far offset from the gameobject should it be (in world space)
    public Vector2 boxOffset;

    //string to store the json text
    private string json;
    //DialogueGraph to store the deserialised json
    private DialogueGraph graph;

    private PlayerActions playerActions;
    private bool inRange = false;
    [HideInInspector]
    public bool rangeToggle = false;

    public float range = 2f;
    private GameObject player;

    [Space()]
    public GameObject graphic;
    private Animator animator;
    public bool facePlayer = true;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

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

        playerActions = ControlManager.GetPlayerActions();
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


        if (inRange)
        {
            if (rangeToggle)
                DialogueBox.instance.ShowIcon(true, this);

            rangeToggle = false;

            //If interact buttons is pressed in range...and the player "can move" (can perform actions outside of UI)
            if (playerActions.Interact.WasPressed && graph != null && GameManager.instance.CanDoActions)
            {
                //Stop them moving and open dialogue
                GameManager.instance.gameRunning = false;
                DialogueBox.instance.ShowDialogue(graph, transform.position + (Vector3)boxOffset, animator, windowColor);

                //If desired, face the player while speaking
                if (facePlayer && graphic)
                {
                    //Make x scale -1 or 1 to flip the sprite to face the player
                    Vector3 scale = graphic.transform.localScale;
                    scale.x = Mathf.Sign(player.transform.position.x - transform.position.x);
                    graphic.transform.localScale = scale;
                }
            }
        }
        else
        {
            if(!rangeToggle)
                DialogueBox.instance.ShowIcon(false);

            rangeToggle = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
