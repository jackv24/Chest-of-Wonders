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
    public RectTransform optionPanel;

    [Space()]
    public Text dialogueText;
    public Button initialButton;
    private List<Button> buttons = new List<Button>();

    private bool dialogueOpen = false;
    private bool waitingForChoice = false;
    private bool waitingForInput = false;

    private Story currentStory;

    private PlayerActions playerActions;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        //Initial button is used as reference and should not be displayed
        initialButton.gameObject.SetActive(false);

        speakerPanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);
    }

    public void OpenDialogue(TextAsset jsonText, string startSpeakerName)
    {
        if (!dialogueOpen)
        {
            currentStory = new Story(jsonText.text);

            StartCoroutine("RunDialogue", string.Format("{0}_start", startSpeakerName));
        }
    }

    IEnumerator RunDialogue(string skipToKnot)
    {
        dialogueOpen = true;

        speakerPanel.gameObject.SetActive(true);
        optionPanel.gameObject.SetActive(true);

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
            string text = currentStory.currentText.TrimEnd(System.Environment.NewLine.ToCharArray());

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

                //Disable all buttons
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].gameObject.SetActive(false);

                //Add any new buttons that are needed
                for(int i = buttons.Count; i < currentStory.currentChoices.Count; i++)
                    buttons.Add(((GameObject)Instantiate(initialButton.gameObject, initialButton.transform.parent)).GetComponent<Button>());

                //Update buttons
                for(int i = 0; i < buttons.Count; i++)
                {
                    SetupButtonEvents(buttons[i], i);

                    Text buttonText = buttons[i].GetComponentInChildren<Text>();

                    if(buttonText)
                        buttonText.text = currentStory.currentChoices[i].text;

                    buttons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                waitingForInput = true;
            }

            while(waitingForChoice)
            {
                yield return new WaitForEndOfFrame();
            }

            while(waitingForInput)
            {
                yield return new WaitForEndOfFrame();

                if (playerActions.Interact.WasPressed || playerActions.Submit.WasPressed)
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
    }

    void SetupButtonEvents(Button button, int index)
    {
        ButtonEventWrapper buttonEvents = button.GetComponent<ButtonEventWrapper>();

        if (buttonEvents)
        {
            ButtonEventWrapper initialButtonEvents = initialButton.GetComponent<ButtonEventWrapper>();

            //Reset button events back to default
            ButtonEventWrapper.CopyEvents(ref buttonEvents, ref initialButtonEvents);

            buttonEvents.onSubmit += delegate
            {
                waitingForChoice = false;

                currentStory.ChooseChoiceIndex(index);
            };
        }
    }
}
