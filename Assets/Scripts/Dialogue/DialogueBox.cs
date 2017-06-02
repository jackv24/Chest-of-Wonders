using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ink.Runtime;

public class DialogueBox : MonoBehaviour
{
    public static DialogueBox Instance;

    public RectTransform speakerPanel;
    public Vector2 speakerPanelOffset;
    public RectTransform optionPanel;
    public Vector2 optionPanelOffset;

    private KeepWorldPosOnCanvas dialoguePos;
    private KeepWorldPosOnCanvas optionsPos;

    [Space()]
    public Text nameText;
    public Image accent;
    public Text dialogueText;
    public Button initialButton;
    private List<Button> buttons = new List<Button>();

    private bool dialogueOpen = false;
    private bool waitingForChoice = false;
    private bool waitingForInput = false;

    private Story currentStory;
    private DialogueSpeaker currentSpeaker;

    private PlayerActions playerActions;

    private void Awake()
    {
        Instance = this;

        dialoguePos = speakerPanel.GetComponent<KeepWorldPosOnCanvas>();
        optionsPos = optionPanel.GetComponent<KeepWorldPosOnCanvas>();
    }

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        //Initial button is used as reference and should not be displayed
        initialButton.gameObject.SetActive(false);

        speakerPanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);

        //Set up delegate functions for getting updated world position for dialogue box elements
        if (dialoguePos)
            dialoguePos.OnGetWorldPos += delegate
            {
                int multiplier = currentSpeaker.transform.position.x < GameManager.instance.player.transform.position.x ? -1 : 1;

                Vector2 s = speakerPanelOffset;
                s.x *= multiplier;

                dialoguePos.worldPos = (Vector2)currentSpeaker.transform.position + currentSpeaker.boxOffset + s;
            };

        if (optionsPos)
            optionsPos.OnGetWorldPos += delegate
            {
                int multiplier = currentSpeaker.transform.position.x < GameManager.instance.player.transform.position.x ? -1 : 1;

                Vector2 o = optionPanelOffset;
                o.x *= multiplier;

                optionsPos.worldPos = (Vector2)GameManager.instance.player.transform.position + o;
            };
    }

    public void OpenDialogue(TextAsset jsonText, string startSpeakerName)
    {
        if (!dialogueOpen)
        {
            GameManager.instance.gameRunning = false;

            currentStory = new Story(jsonText.text);

            StartCoroutine("RunDialogue", string.Format("{0}_start", startSpeakerName));
        }
    }

    IEnumerator RunDialogue(string skipToKnot)
    {
        dialogueOpen = true;

        speakerPanel.gameObject.SetActive(true);

        //Go straight to speaker knot if there is one
        try
        {
            currentStory.ChoosePathString(skipToKnot.ToLower());
        }
        catch { }

        while(currentStory.canContinue)
        {
            currentStory.Continue();
            int index = 0;
            dialogueText.text = "";

            //Remove newline from end of string
            string text = currentStory.currentText;//.Replace("\n", "");

            //Parse text for speaker (removing it in the process)
            text = ParseSpeaker(text);

            while (index < text.Length)
            {
                //Split text into shown and hidden text
                string showText = text.Remove(index, text.Length - index);
                string hideText = text.Remove(0, index);

                //Set text with colour to hide
                dialogueText.text = string.Format("{0}{1}{2}{3}", showText, "<color=#FFFFFF00>", hideText, "</color>");

                index++;

                yield return new WaitForSeconds(0.05f);
            }

            if(currentStory.currentChoices.Count > 0)
            {
                waitingForChoice = true;
                optionPanel.gameObject.SetActive(true);

                //Disable all buttons
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].gameObject.SetActive(false);

                //Add any new buttons that are needed
                for(int i = buttons.Count; i < currentStory.currentChoices.Count; i++)
                    buttons.Add(((GameObject)Instantiate(initialButton.gameObject, initialButton.transform.parent)).GetComponent<Button>());

                //Update buttons
                for(int i = 0; i < currentStory.currentChoices.Count; i++)
                {
                    SetupButtonEvents(buttons[i], i);

                    Text buttonText = buttons[i].GetComponentInChildren<Text>();

                    if(buttonText)
                        buttonText.text = currentStory.currentChoices[i].text;

                    buttons[i].gameObject.SetActive(true);
                }

                EventSystem.current.firstSelectedGameObject = null;
                EventSystem.current.SetSelectedGameObject(null);

                EventSystem.current.firstSelectedGameObject = buttons[0].gameObject;
                EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
            }
            else
            {
                waitingForInput = true;

                optionPanel.gameObject.SetActive(false);
            }

            while(waitingForChoice)
            {
                yield return new WaitForEndOfFrame();
            }

            while(waitingForInput)
            {
                yield return new WaitForEndOfFrame();

                if (playerActions.Interact.WasPressed || playerActions.Submit.WasPressed || playerActions.Jump.WasPressed || Input.GetMouseButton(0))
                    waitingForInput = false;
            }

            //Disable all buttons
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        speakerPanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);

        dialogueOpen = false;

        GameManager.instance.gameRunning = true;
    }

    void SetupButtonEvents(Button button, int index)
    {
        ButtonEventWrapper buttonEvents = button.GetComponent<ButtonEventWrapper>();

        if (buttonEvents)
        {
            ButtonEventWrapper initialButtonEvents = initialButton.GetComponent<ButtonEventWrapper>();

            //Reset button events back to default
            ButtonEventWrapper.CopyEvents(ref initialButtonEvents, ref buttonEvents);

            buttonEvents.onSubmit += delegate
            {
                waitingForChoice = false;
                
                currentStory.ChooseChoiceIndex(index);
            };
        }
    }

    string ParseSpeaker(string text)
    {
        char[] chars = text.ToCharArray();

        string speakerName = "";
        string outputString = "";
        bool readingName = true;

        //If sentence starts with '@' it should follow with a name
        if (chars[0] == '@')
        {
            for(int i = 1; i < chars.Length; i++)
            {
                //Read name until ':' is reached (excluding any following space)
                if (chars[i] == ':')
                {
                    i++;
                    readingName = false;

                    if (chars[i] == ' ')
                        i++;
                }

                //Save name part of string into name string, and rest into output string
                if (readingName)
                    speakerName += chars[i];
                else
                    outputString += chars[i];
            }

            ///Update dialogue box with speaker details
            //Update name text
            if(nameText)
                nameText.text = speakerName;

            //Find all Dialogue Speakers in scene
            DialogueSpeaker[] speakers = FindObjectsOfType<DialogueSpeaker>();

            DialogueSpeaker speaker = null;

            //Loop through all speakers to find one matching the speaker name
            foreach(DialogueSpeaker s in speakers)
            {
                if (s.gameObject.name.ToLower() == speakerName.ToLower())
                    speaker = s;
            }

            //If a matching speaker was found
            if(speaker)
            {
                //Change window accent colour to match speaker
                if(accent)
                    accent.color = speaker.windowColor;

                currentSpeaker = speaker;
            }

            return outputString;
        }
        else
            return text;
    }
}
