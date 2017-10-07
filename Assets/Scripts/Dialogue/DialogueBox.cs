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

	[Space()]
	public Animator speakerPanelAnimator;
	public AnimationClip speakerOpenAnim;
	public AnimationClip speakerCloseAnim;

    [Space()]
    public GameObject interactIcon;

    private KeepWorldPosOnCanvas dialoguePos;
    private KeepWorldPosOnCanvas optionsPos;

    [Space()]
    public Text nameText;
    public Image accent;
    public Text dialogueText;
    public Image trail;
    public int trailOffset = 10;
    public Button initialButton;
    private List<Button> buttons = new List<Button>();

    private bool dialogueOpen = false;
    private bool waitingForChoice = false;
    private bool waitingForInput = false;

    private bool buttonPressed = false;
	private bool autoContinue = false;
	private float pauseTime = 0;

    [Space()]
    public float textSpeed = 20;
    public float fastTextSpeed = 50;

	[Space()]
	public float optionSelectDelay = 0.5f;

    private Story currentStory;
    private TextAsset textAsset;
    private DialogueSpeaker currentSpeaker;
    private List<string> animParams = new List<string>();

    private PlayerActions playerActions;

    private DialogueSounds sounds;

	private bool hidden = false;

    private void Awake()
    {
        Instance = this;

        dialoguePos = speakerPanel.GetComponent<KeepWorldPosOnCanvas>();
        optionsPos = optionPanel.GetComponent<KeepWorldPosOnCanvas>();
        sounds = GetComponent<DialogueSounds>();
    }

    private void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        //Initial button is used as reference and should not be displayed
        initialButton.gameObject.SetActive(false);

        speakerPanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);

        interactIcon.SetActive(false);

        //Set up delegate functions for getting updated world position for dialogue box elements
        if (dialoguePos)
        {
            dialoguePos.OnGetWorldPos += delegate
            {
                int multiplier = currentSpeaker.transform.position.x < GameManager.instance.player.transform.position.x ? -1 : 1;

                Vector2 s = speakerPanelOffset;
                s.x *= multiplier;

                dialoguePos.worldPos = (Vector2)currentSpeaker.transform.position + currentSpeaker.boxOffset + s;

                if(trail)
                {
                    Vector3 pos = trail.rectTransform.localPosition;
                    pos.x = trailOffset * multiplier;
                    trail.rectTransform.localPosition = pos;

                    Vector3 scale = trail.rectTransform.localScale;
                    scale.x = multiplier;
                    trail.rectTransform.localScale = scale;
                }
            };
        }

        if (optionsPos)
        {
            optionsPos.OnGetWorldPos += delegate
            {
                int multiplier = 1;

                CharacterMove move = GameManager.instance.player.GetComponent<CharacterMove>();

                if (move)
                {
                    multiplier = move.FacingDirection > 0 ? -1 : 1;
                    optionPanel.pivot = new Vector2(move.FacingDirection > 0 ? 0 : 1, optionPanel.pivot.y);
                }

                Vector2 o = optionPanelOffset;
                o.x *= multiplier;

                optionsPos.worldPos = (Vector2)GameManager.instance.player.transform.position + o;
            };
        }
    }

    void Update()
    {
        //Get button press in update to sync with InControl
        if (playerActions.Interact.WasPressed || playerActions.Submit.WasPressed || playerActions.Jump.WasPressed || Input.GetMouseButtonDown(0))
            buttonPressed = true;
    }

    public void OpenDialogue(TextAsset jsonText, string startSpeakerName)
    {
        if (!dialogueOpen)
        {
            GameManager.instance.gameRunning = false;

            HidePromptIcon();

            currentStory = new Story(jsonText.text);

            string json = SaveManager.instance.LoadDialogueJson(jsonText.name);

            if (json != "")
                currentStory.state.LoadJson(json);

            textAsset = jsonText;

            StartCoroutine("RunDialogue", string.Format("{0}_start", startSpeakerName));
        }
    }

    IEnumerator RunDialogue(string skipToKnot)
    {
        BindExternalFunctions();

        dialogueOpen = true;

        buttonPressed = false;

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
            string text = currentStory.currentText;

            //Parse text for speaker (removing it in the process)
            text = ParseSpeaker(text);

            HandleTags(currentStory.currentTags);

            //Remove end line
            text = text.Replace("\n", "");

			//Before opening, set new text and as invisible for sizing box correctly
			dialogueText.text = string.Format("{0}{1}{2}", "<color=#FFFFFF00>", text, "</color>");

			//Play open animation
			if (hidden)
			{
				if (speakerPanelAnimator && speakerOpenAnim)
				{
					speakerPanelAnimator.Play(speakerOpenAnim.name);
					yield return new WaitForSeconds(speakerOpenAnim.length);
				}

				hidden = false;
			}

			bool speedPressed = false;

			if (text.Replace(" ", "").Length > 0)
			{
				while (index < text.Length)
				{
					//Split text into shown and hidden text
					string showText = text.Remove(index, text.Length - index);
					string hideText = text.Remove(0, index);

					//Set text with colour to hide
					dialogueText.text = string.Format("{0}{1}{2}{3}", showText, "<color=#FFFFFF00>", hideText, "</color>");

					index++;

					//Play one "blip" sound for every character printed
					if (sounds)
						sounds.PlaySound("blip");

					yield return new WaitForSeconds(!speedPressed ? 1 / textSpeed : 1 / fastTextSpeed);

					if (!speedPressed && buttonPressed)
					{
						buttonPressed = false;
						speedPressed = true;
					}
				}
			}
			else
			{
				if (!hidden)
				{
					hidden = true;

					if (speakerPanelAnimator && speakerCloseAnim)
					{
						speakerPanelAnimator.Play(speakerCloseAnim.name);
						yield return new WaitForSeconds(speakerCloseAnim.length);
					}
				}
			}

            dialogueText.text = text;

            if (currentStory.currentChoices.Count > 0)
            {
                waitingForChoice = true;

				KeepWorldPosOnCanvas keepPos = optionsPos.GetComponent<KeepWorldPosOnCanvas>();
				if (keepPos)
					keepPos.GetWorldPos();

                optionPanel.gameObject.SetActive(true);

                //Disable all buttons
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].gameObject.SetActive(false);

                //Add any new buttons that are needed
                for(int i = buttons.Count; i < currentStory.currentChoices.Count; i++)
                    buttons.Add(((GameObject)Instantiate(initialButton.gameObject, initialButton.transform.parent)).GetComponent<Button>());

				foreach (Button b in buttons)
					b.interactable = false;

				//Update buttons
				for (int i = 0; i < currentStory.currentChoices.Count; i++)
                {
                    SetupButtonEvents(buttons[i], i);

                    Text buttonText = buttons[i].GetComponentInChildren<Text>();

                    if(buttonText)
                        buttonText.text = currentStory.currentChoices[i].text;

                    buttons[i].gameObject.SetActive(true);
                }

                EventSystem.current.firstSelectedGameObject = null;
                EventSystem.current.SetSelectedGameObject(null);

				foreach (Button b in buttons)
				{
					yield return new WaitForSeconds(optionSelectDelay);
					b.interactable = true;
				}

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
            buttonPressed = false;

            while(waitingForInput)
            {
                yield return new WaitForEndOfFrame();

                if (buttonPressed)
                {
                    buttonPressed = false;
                    waitingForInput = false;
                }
            }

            //Disable all buttons
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].gameObject.SetActive(false);

			yield return new WaitForSeconds(pauseTime);
		}

        yield return new WaitForEndOfFrame();

        //speakerPanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);

        dialogueOpen = false;

        ShowSpeakerTalking(false);

        GameManager.instance.gameRunning = true;
        if(!Application.isEditor) Cursor.visible = false;

        currentSpeaker.rangeToggle = true;

        //Make sure speaker params are cleared
        ClearParams(currentSpeaker.GetComponentInChildren<Animator>());

        SaveManager.instance.SaveDialogueJson(textAsset.name, currentStory.state.ToJson());

		//Close speaker panel at end to avoid hanging up other things
		if (speakerPanelAnimator && speakerCloseAnim)
		{
			speakerPanelAnimator.Play(speakerCloseAnim.name);
			yield return new WaitForSeconds(speakerCloseAnim.length);
			speakerPanel.gameObject.SetActive(false);
		}
		else
			speakerPanel.gameObject.SetActive(false);
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

				optionPanel.gameObject.SetActive(false);
            };
        }
    }

    void BindExternalFunctions()
    {
        currentStory.BindExternalFunction("move", (float x) =>
        {
            //Moves the speaker x number of metres horizontally
            currentSpeaker.MoveSpeaker(x);
        });

		currentStory.BindExternalFunction("hasItem", (string x) =>
		{
			PlayerInventory inventory = GameManager.instance.player.GetComponent<PlayerInventory>();

			if (inventory)
			{
				return inventory.CheckItem(x);
			}
			else
				return false;
		});

		currentStory.BindExternalFunction("setFlag", (string x) =>
		{
			SaveManager.instance.SetFlag(x);
		});

		currentStory.BindExternalFunction("checkFlag", (string x) =>
		{
			return SaveManager.instance.CheckFlag(x);
		});
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

                ShowSpeakerTalking(false);

                currentSpeaker = speaker;

                ShowSpeakerTalking(true);
            }

            return outputString;
        }
        else
            return text;
    }

    public void ClearParams(Animator anim)
    {
        //Clear previous anim parameters
        if (anim)
        {
            foreach (string param in animParams)
            {
                anim.SetBool(param, false);
            }
        }

        animParams.Clear();
    }

    public void HandleTags(List<string> tags)
    {
        Animator anim = currentSpeaker.GetComponentInChildren<Animator>();

        ClearParams(anim);

		//Reset values that may have been set in previous tags
		autoContinue = false;
		pauseTime = 0;

		//Handle tags
		foreach (string tag in tags)
        {
            if(tag.Contains("animation_"))
            {
                string parameter = tag.Replace("animation_", "");
                animParams.Add(parameter);

                if(anim)
                    anim.SetBool(parameter, true);
            }
            else if (tag.Contains("sound_"))
            {
                string parameter = tag.Replace("sound_", "");

                if (sounds)
                    sounds.PlaySound(parameter);
            }
			else if(tag == "auto continue")
			{
				autoContinue = true;
			}
			else if(tag.Contains("pause"))
			{
				string text = tag.Replace("pause ", "");

				float time = 0;

				if (!float.TryParse(text, out time))
					Debug.LogError(text + " is not a valid float value to pause for!");

				pauseTime = time;
			}
            else
            {
                Debug.LogWarning("Encountered unknown tag in dialogue: " + tag);
            }
        }
    }

    void ShowSpeakerTalking(bool value)
    {
        if (currentSpeaker)
        {
            Animator anim = currentSpeaker.GetComponentInChildren<Animator>();

            if (anim)
                anim.SetBool("isTalking", value);
        }
    }

    public void ShowPromptIcon(Vector2 position)
    {
        if(interactIcon)
        {
            interactIcon.transform.position = position;
            interactIcon.SetActive(true);
        }
    }

    public void HidePromptIcon()
    {
        if (interactIcon)
            interactIcon.SetActive(false);
    }

	public void AutoContinue()
	{
		if (autoContinue)
			waitingForInput = false;
	}
}
