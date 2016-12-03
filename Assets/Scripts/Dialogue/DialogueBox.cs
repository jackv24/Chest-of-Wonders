using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueBox : MonoBehaviour
{
    //Static instance for easy access
    public static DialogueBox instance;

    public Text nameText;
    public Text dialogueText;

    [Space()]
    public GameObject button;
    private Button[] buttons;
    private int selectedButton = 0;

    public float textSpeed = 2f;

    private Vector3 worldPos;

    private PlayerActions playerActions;

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
        playerActions = new PlayerActions();

        InControl.InControlInputModule inputModule = GameObject.Find("EventSystem").GetComponent<InControl.InControlInputModule>();
        if (inputModule != null)
        {
            inputModule.SubmitAction = playerActions.Submit;
        }

        //Hide by default
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //Navigate buttons with actions
        if (playerActions.Left.WasPressed && buttons.Length > 0)
        {
            selectedButton = selectedButton > 0 ? selectedButton - 1 : buttons.Length - 1;
            EventSystem.current.SetSelectedGameObject(buttons[selectedButton].gameObject);
        }
        else if (playerActions.Right.WasPressed && buttons.Length > 0)
        {
            selectedButton = selectedButton < buttons.Length - 1 ? selectedButton + 1 : 0;
            EventSystem.current.SetSelectedGameObject(buttons[selectedButton].gameObject);
        }
        //Close dialogue if no buttons present, and action is pressed
        else if (playerActions.Submit.WasPressed && buttons.Length <= 0)
        {
            GameManager.instance.canMove = true;
            gameObject.SetActive(false);
        }
        else if (playerActions.Attack1.WasPressed && buttons.Length <= 0)
        {
            GameManager.instance.canMove = true;
            gameObject.SetActive(false);
        }
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

    public void UpdateDialogue(DialogueGraph graph, int nodeID)
    {
        //Get node from graph based on index
        DialogueGraph.DialogueGraphNode node = graph.GetNode(nodeID);

        //Get all buttons in the dialogue
        buttons = button.transform.parent.GetComponentsInChildren<Button>();
        selectedButton = 0;

        foreach (Button button in buttons)
        {
            //Disable end reset button listeners for re-use
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        //Set dialogue text
        nameText.text = graph.speakerName;

        //Loop through any options
        for (int i = 0; i < node.options.Count; i++)
        {
            //Create copy of variable to fix "capturing" error with delegates
            int n = i;

            if (graph.GetNode(node.options[i].target) != null)
            {
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
        }

        //Get buttons for input
        buttons = button.transform.parent.GetComponentsInChildren<Button>();

        EventSystem.current.SetSelectedGameObject(null);

        //Show the dialogue
        gameObject.SetActive(true);

        StartCoroutine("DisplayTextOverTime", node.text);
    }

    IEnumerator DisplayTextOverTime(string text)
    {
        char[] chars = (text).ToCharArray();

        Transform buttonGroup = button.transform.parent;

        dialogueText.text = "";

        for (int i = 0; i < chars.Length; i++)
        {
            yield return new WaitForSeconds(1 / textSpeed);

            dialogueText.text += chars[i];
        }
    }
}
