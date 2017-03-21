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

    public Image[] accents;

    [Space()]
    public GameObject button;
    private Button[] buttons;
    private int selectedButton = 0;

    public RectTransform buttonsRect;
    public AnimationCurve buttonsFoldoutCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public float foldoutLength = 0.25f;

    public float textSpeed = 2f;
    private bool textDonePrinting = false;
    private string targetText;

    public GameObject openIcon;

    private Vector3 worldPos;

    private PlayerActions playerActions;
    private Animator speakerAnimator;

    //Keep track of next node for when there is no button to setup listeners
    private DialogueGraph currentGraph;
    private int nextNode = -1;

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
        if ((playerActions.Left.WasPressed || playerActions.Up.WasPressed) && buttons.Length > 0 && textDonePrinting)
        {
            selectedButton = selectedButton > 0 ? selectedButton - 1 : buttons.Length - 1;
            EventSystem.current.SetSelectedGameObject(buttons[selectedButton].gameObject);
        }
        else if ((playerActions.Right.WasPressed || playerActions.Down.WasPressed) && buttons.Length > 0 && textDonePrinting)
        {
            selectedButton = selectedButton < buttons.Length - 1 ? selectedButton + 1 : 0;
            EventSystem.current.SetSelectedGameObject(buttons[selectedButton].gameObject);
        }
        //Close dialogue if no buttons present, and action is pressed
        else if (playerActions.Submit.WasPressed || playerActions.MeleeAttack.WasPressed || Input.GetMouseButtonDown(0))
        {
            //Only go to next node if text is done printing, and there are no buttons
            if (textDonePrinting && buttons.Length <= 0)
            {
                //If there is no next node, dialogue has finished
                if (nextNode < 0)
                {
                    GameManager.instance.gameRunning = true;
                    ShowIcon(true);
                    gameObject.SetActive(false);

                    if (speakerAnimator)
                        speakerAnimator.SetBool("isTalking", false);
                }
                else
                {
                    UpdateDialogue(currentGraph, nextNode);
                }
            }
            //If text is not done, then skip to end
            else
            {
                StopCoroutine("DisplayTextOverTime");
                dialogueText.text = targetText;
                textDonePrinting = true;
            }
        }
    }

    private void LateUpdate()
    {
        //Keep world position
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    public void ShowIcon(bool value)
    {
        if (openIcon)
        {
            openIcon.GetComponent<Animator>().SetBool("isVisible", value);
        }
    }

    public void ShowIcon(bool value, DialogueSpeaker speaker)
    {
        if (openIcon)
        {
            if (value)
            {
                openIcon.transform.position = (Vector2)speaker.transform.position + speaker.boxOffset;
                openIcon.GetComponent<Animator>().SetBool("isVisible", true);
            }
            else
                openIcon.GetComponent<Animator>().SetBool("isVisible", false);
        }
    }

    public void ShowDialogue(DialogueGraph graph, Vector3 worldPos, Color color)
    {
        ShowIcon(false);

        //Set world position
        this.worldPos = worldPos;

        currentGraph = graph;

        foreach (Image img in accents)
            img.color = color;

        //Start dialogue at first node
        UpdateDialogue(graph, 0);
    }

    public void ShowDialogue(DialogueGraph graph, Vector3 worldPos, Animator speakerAnimator, Color color)
    {
        this.speakerAnimator = speakerAnimator;

        if(speakerAnimator)
            speakerAnimator.SetBool("isTalking", true);

        ShowDialogue(graph, worldPos, color);
    }

    public void UpdateDialogue(DialogueGraph graph, int nodeID)
    {
        //Get node from graph based on index
        DialogueGraph.DialogueGraphNode node = graph.GetNode(nodeID);

        //Get all buttons in the dialogue
        buttons = button.transform.parent.GetComponentsInChildren<Button>(true);
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

        if (node.options.Count <= 0 && node.nextNode >= 0)
            nextNode = node.nextNode;
        else
            nextNode = -1;

        //Get buttons for input
        buttons = button.transform.parent.GetComponentsInChildren<Button>();

        EventSystem.current.SetSelectedGameObject(null);

        //Show the dialogue
        gameObject.SetActive(true);

        string text = node.text;

        //Parse text for commands
        if (text.Contains("<save>"))
        {
            SaveManager.instance.SaveGame(true);
            text = text.Replace("<save>", "");
        }

        //Start printing text one character at a time
        StartCoroutine("DisplayTextOverTime", text);

        //If there are buttons, let them fold out
        if (buttons.Length > 0)
            StartCoroutine("FoldoutOptions");
    }

    IEnumerator DisplayTextOverTime(string text)
    {
        textDonePrinting = false;
        targetText = text;

        char[] chars = (text).ToCharArray();

        Transform buttonGroup = button.transform.parent;

        dialogueText.text = "";

        for (int i = 0; i < chars.Length; i++)
        {
            yield return new WaitForSeconds(1 / textSpeed);

            dialogueText.text += chars[i];
        }

        textDonePrinting = true;
    }

    IEnumerator FoldoutOptions()
    {
        float timeElapsed = 0;

        while (timeElapsed < foldoutLength)
        {
            //Evaluate and set scale
            Vector3 scale = buttonsRect.localScale;
            scale.y = buttonsFoldoutCurve.Evaluate(timeElapsed / foldoutLength);
            buttonsRect.localScale = scale;

            //Do not fold out until text is done printing
            while (!textDonePrinting)
                yield return new WaitForEndOfFrame();

            //Wait one frame and count time elapsed
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }
    }
}
