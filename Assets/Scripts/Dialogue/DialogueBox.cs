using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NodeCanvas.DialogueTrees;

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
    private bool waitingForInput = false;

    private bool buttonPressed = false;
	private bool autoContinue = false;
	private float pauseTime = 0;

    [Space()]
    public float textSpeed = 20;
    public float fastTextSpeed = 50;

	[Space()]
	public float optionSelectDelay = 0.5f;

	private DialogueSpeaker currentSpeaker;

    private PlayerActions playerActions;

	private bool hidden = false;

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

		//Subscribe to DialogueTree events
		DialogueTree.OnDialogueStarted += OnDialogueStarted;
		DialogueTree.OnDialogueFinished += OnDialogueFinished;
		DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
		DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
    }

    void Update()
    {
		//Get button press in update to sync with InControl
		if (playerActions.Interact.WasPressed || playerActions.Submit.WasPressed || playerActions.Jump.WasPressed || Input.GetMouseButtonDown(0))
			buttonPressed = true;
		else
			waitingForInput = true;
    }

    public void OnDialogueStarted(DialogueTree dialogueTree)
    {
        if (!dialogueOpen)
        {
			dialogueOpen = true;

            GameManager.instance.gameRunning = false;

            HidePromptIcon();

			speakerPanel.gameObject.SetActive(true);

			if (speakerPanelAnimator && speakerOpenAnim)
				speakerPanelAnimator.Play(speakerOpenAnim.name);

			//Load blackboard variables
			string key = GameManager.instance.loadedSceneIndex + "_" + dialogueTree.name.Replace(" DialogueTree", "");
			string json;

			if (SaveManager.instance.GetBlackboardJson(key, out json))
			{
				dialogueTree.DeserializeLocalBlackboard(json);
				Debug.Log("Loaded blackboard: " + key);
			}
		}
    }

	public void OnDialogueFinished(DialogueTree dialogueTree)
	{
		if (dialogueOpen)
		{
			dialogueOpen = false;

			GameManager.instance.gameRunning = true;

			speakerPanel.gameObject.SetActive(false);

			if (speakerPanelAnimator && speakerCloseAnim)
				speakerPanelAnimator.Play(speakerCloseAnim.name);

			//Save blackboard variables
			if(dialogueTree.blackboard.variables.Count > 0)
			{
				string key = GameManager.instance.loadedSceneIndex + "_" + dialogueTree.name.Replace(" DialogueTree", "");
				string json = dialogueTree.SerializeLocalBlackboard();

				SaveManager.instance.SaveBlackBoardJson(key, json);
			}
		}
	}

	void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
	{
		KeepWorldPosOnCanvas keepPos = optionsPos.GetComponent<KeepWorldPosOnCanvas>();
		if (keepPos)
			keepPos.GetWorldPos();

		optionPanel.gameObject.SetActive(true);

		//Disable all buttons
		for (int i = 0; i < buttons.Count; i++)
			buttons[i].gameObject.SetActive(false);

		//Add any new buttons that are needed
		for (int i = buttons.Count; i < info.options.Count; i++)
			buttons.Add(((GameObject)Instantiate(initialButton.gameObject, initialButton.transform.parent)).GetComponent<Button>());

		foreach (Button b in buttons)
			b.interactable = false;

		//Update buttons
		foreach(KeyValuePair<IStatement, int> pair in info.options)
		{
			SetupButtonEvents(buttons[pair.Value], info, pair.Value);

			Text buttonText = buttons[pair.Value].GetComponentInChildren<Text>();

			if (buttonText)
				buttonText.text = pair.Key.text;

			buttons[pair.Value].gameObject.SetActive(true);
		}

		EventSystem.current.firstSelectedGameObject = null;
		EventSystem.current.SetSelectedGameObject(null);

		foreach (Button b in buttons)
		{
			//TODO: Add delay
			//yield return new WaitForSeconds(optionSelectDelay);
			b.interactable = true;
		}

		EventSystem.current.firstSelectedGameObject = buttons[0].gameObject;
		EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
	}

	void OnSubtitlesRequest(SubtitlesRequestInfo info)
	{
		StartCoroutine(RunSubtitleRequest(info));
	}

	IEnumerator RunSubtitleRequest(SubtitlesRequestInfo info)
	{
		string text = info.statement.text;
		IDialogueActor actor = info.actor;

		if (accent)
			accent.color = actor.dialogueColor;

		if (nameText)
			nameText.text = actor.name;

		if(actor.transform)
			currentSpeaker = actor.transform.GetComponent<DialogueSpeaker>();

		if(dialogueText)
		{
			dialogueText.text = text;
		}

		while(waitingForInput)
		{
			if (buttonPressed)
				waitingForInput = false;

			yield return null;
		}

		info.Continue();
	}

    void SetupButtonEvents(Button button, MultipleChoiceRequestInfo info, int index)
    {
        ButtonEventWrapper buttonEvents = button.GetComponent<ButtonEventWrapper>();

        if (buttonEvents)
        {
            ButtonEventWrapper initialButtonEvents = initialButton.GetComponent<ButtonEventWrapper>();

            //Reset button events back to default
            ButtonEventWrapper.CopyEvents(ref initialButtonEvents, ref buttonEvents);

            buttonEvents.onSubmit += delegate
            {
				info.SelectOption(index);

				optionPanel.gameObject.SetActive(false);
            };
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
