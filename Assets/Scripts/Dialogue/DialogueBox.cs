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

	public bool IsDialogueOpen { get; private set; } = false;

	private bool optionsOpen = false;
    private bool waitingForInput = false;

    private bool buttonPressed = false;
	private bool autoContinue = false;

	[Space()]
    public float textSpeed = 20;
	public int maxCharsBeforeWrap = 30;
	public LayoutElement textPanel;
	public VerticalLayoutGroup textLayoutGroup;
	private float textPreferredWidth;

	[Space()]
	public AudioClip textPrintSound;
	public MinMaxFloat pitchVariance = new MinMaxFloat(0.85f, 1.15f);
	public MinMaxFloat textSoundDelay = new MinMaxFloat(0.2f, 0.3f);
	private AudioSource audioSource;

	[Space()]
	public float optionSelectDelay = 0.5f;

	private DialogueSpeaker currentSpeaker;

    private PlayerActions playerActions;

    private void Awake()
    {
        Instance = this;

		audioSource = GetComponent<AudioSource>();

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
		DialogueTree.OnDialoguePaused += OnDialoguePaused;
		DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
		DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;

		if (textPanel)
			textPreferredWidth = textPanel.preferredWidth;
    }

    void Update()
    {
		if (waitingForInput && !buttonPressed)
		{
			if (playerActions.Interact.WasPressed || playerActions.Submit.WasPressed || playerActions.Jump.WasPressed || Input.GetMouseButtonDown(0))
			{
				waitingForInput = false;
				buttonPressed = true;
			}
		}
		else if (playerActions.Interact.WasReleased || playerActions.Submit.WasReleased || playerActions.Jump.WasReleased || Input.GetMouseButtonUp(0))
		{
			buttonPressed = false;
		}

	}

    public void OnDialogueStarted(DialogueTree dialogueTree)
    {
        if (!IsDialogueOpen)
        {
			IsDialogueOpen = true;

            GameManager.instance.gameRunning = false;

			speakerPanel.gameObject.SetActive(true);

			if (speakerPanelAnimator && speakerOpenAnim)
				speakerPanelAnimator.Play(speakerOpenAnim.name);

			//Load blackboard variables
			string key = GameManager.instance.loadedSceneIndex + "_" + dialogueTree.name.Replace(" DialogueTree", "");
			string json;

			if (SaveManager.instance.GetBlackboardJson(key, out json))
			{
				dialogueTree.DeserializeLocalBlackboard(json);
			}

			//Set camera target
			if (CameraControl.Instance)
				CameraControl.Instance.AddFocusTarget(new CameraControl.FocusTarget(dialogueTree.agent.transform, Vector2.zero));
		}
    }

	public void OnDialogueFinished(DialogueTree dialogueTree)
	{
		if (IsDialogueOpen)
		{
			//Save blackboard variables
			if(dialogueTree.blackboard.variables.Count > 0)
			{
				string key = GameManager.instance.loadedSceneIndex + "_" + dialogueTree.name.Replace(" DialogueTree", "");
				string json = dialogueTree.SerializeLocalBlackboard();

				SaveManager.instance.SaveBlackBoardJson(key, json);
			}

			//Clear camera target
			CameraControl camera = CameraControl.Instance;
			if (camera)
			{
				CameraControl.FocusTarget focusTarget = camera.GetFocusTargetByTransform(dialogueTree.agent.transform);
				if (focusTarget != null)
					camera.RemoveFocusTarget(focusTarget);
			}

			StartCoroutine(EndDialogue());
		}
	}

	IEnumerator EndDialogue()
	{
		//Close panel (wait for anim to finish before disabling)
		if (speakerPanelAnimator && speakerCloseAnim)
			speakerPanelAnimator.Play(speakerCloseAnim.name);

		yield return new WaitForSeconds(speakerCloseAnim.length);

		speakerPanel.gameObject.SetActive(false);

		currentSpeaker.ShowPrompt();

		//Return control
		GameManager.instance.gameRunning = true;

		IsDialogueOpen = false;
	}

	void OnDialoguePaused(DialogueTree dialogueTree)
	{
		HideDialogueBox();
	}

	public void HideDialogueBox()
	{
		if (speakerPanelAnimator && speakerCloseAnim)
			speakerPanelAnimator.Play(speakerCloseAnim.name);

		IsDialogueOpen = false;
	}

	void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
	{
		StartCoroutine(SetupOptions(info));
	}

	IEnumerator SetupOptions(MultipleChoiceRequestInfo info)
	{
		optionsOpen = true;

		yield return null;

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
		foreach (KeyValuePair<IStatement, int> pair in info.options)
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
			yield return new WaitForSeconds(optionSelectDelay);
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
		IDialogueActor actor = info.actor;

		if (actor.transform)
			currentSpeaker = actor.transform.GetComponent<DialogueSpeaker>();

		yield return null;

		if (accent)
			accent.color = actor.dialogueColor;

		if (nameText)
			nameText.text = actor.name;

		if(dialogueText)
		{
			bool withSound = !info.statement.meta.ToLower().Contains("nosound");

			//Fixes layout issues
			if (info.statement.text.Length > maxCharsBeforeWrap)
			{
				if (textPanel)
					textPanel.preferredWidth = textPreferredWidth;

				if (textLayoutGroup)
					textLayoutGroup.childControlHeight = true;
			}
			else
			{
				if(textPanel)
					textPanel.preferredWidth = -1;

				if (textLayoutGroup)
					textLayoutGroup.childControlHeight = false;
			}

			//Open dialogue box with animation if it is closed
			if (!IsDialogueOpen)
			{
				IsDialogueOpen = true;

				//Set dialogue text and as invisible to get correct box size
				dialogueText.text = $"<color=#FFFFFF00>{info.statement.text}</color>";

				if (speakerPanelAnimator && speakerOpenAnim)
					speakerPanelAnimator.Play(speakerOpenAnim.name);

				yield return new WaitForSeconds(speakerOpenAnim.length);
			}

			yield return StartCoroutine(PrintOverTime(dialogueText, info.statement.text, withSound));
		}

		//MultipleChoiceRequest handles continuing if there are options
		if (!optionsOpen)
		{
			waitingForInput = true;
			while (waitingForInput)
			{
				yield return null;
			}

			info.Continue();
		}
	}

    IEnumerator PrintOverTime(Text textObj, string text, bool withSound)
    {
        Coroutine soundRoutine = null;

        if (withSound)
            soundRoutine = StartCoroutine(PlayTextSounds());

        waitingForInput = true;

        int charCount = text.Length;
        for (int i = 0; i < charCount; i++)
        {
            if (!waitingForInput)
                break;

            string showTex = text.Remove(i, charCount - i);
            string hideText = text.Remove(0, i);

            textObj.text = string.Format("{0}<color=#FFFFFF00>{1}</color>", showTex, hideText);

            yield return new WaitForSeconds(1 / textSpeed);
        }

        textObj.text = text;

        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
    }

    IEnumerator PlayTextSounds()
	{
		if(audioSource && textPrintSound)
		{
			audioSource.clip = textPrintSound;

			while(true)
			{
				audioSource.pitch = pitchVariance.RandomValue;
				audioSource.Play();

				yield return new WaitForSeconds(textSoundDelay.RandomValue);
			}
		}
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
				optionsOpen = false;

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

	public void AutoContinue()
	{
		if (autoContinue)
			waitingForInput = false;
	}
}
