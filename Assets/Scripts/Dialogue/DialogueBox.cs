using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    //Static instance for easy access
    public static DialogueBox instance;

    public Text nameText;
    public Text dialogueText;

    [Space()]
    public GameObject button;

    private Vector3 worldPos;

    private void Awake()
    {
        //There should only be one dialogue box present in the scene
        if (!instance)
            instance = this;
        else
        {
            Debug.LogWarning("More than one Speech Dialog was found in the scene, and has been removed.");

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Hide by default
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //Keep world position
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    public void ShowDialogue(DialogueGraph graph, Vector3 worldPos)
    {
        //Set world position
        this.worldPos = worldPos;

        //Start dialogue at first node
        UpdateDialogue(graph, 0);
    }

    public void UpdateDialogue(DialogueGraph graph, int nodeIndex)
    {
        //Get node from graph based on index
        DialogueGraph.DialogueGraphNode node = graph.nodes[nodeIndex];

        //Get all buttons in the dialogue
        Button[] buttons = button.transform.parent.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            //Disable end reset button listeners for re-use
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        //Set dialogue text
        nameText.text = graph.speakerName;
        dialogueText.text = node.text;

        //Loop through any options
        for (int i = 0; i < node.options.Count; i++)
        {
            //Create copy of variable to fix "capturing" error with delegates
            int n = i;

            //if option is within already existing buttons, re-use buttons
            if (i < buttons.Length)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = node.options[i].text;
                //Add click listener to target
                buttons[i].onClick.AddListener(delegate { UpdateDialogue(graph, node.options[n].target); });
            }
            //If extra buttons are required, instantiate them
            else
            {
                GameObject obj = Instantiate(button, button.transform.parent);
                obj.SetActive(true);
                obj.GetComponentInChildren<Text>().text = node.options[i].text;
                //Add click listener to target
                obj.GetComponent<Button>().onClick.AddListener(delegate { UpdateDialogue(graph, node.options[n].target); });
            }
        }

        //Show the dialogue
        gameObject.SetActive(true);
    }
}
