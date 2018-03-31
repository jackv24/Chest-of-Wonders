using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractType
{
	Speak,
	Open,
}

public interface IInteractible
{
	InteractType InteractType { get; }

	void Interact();
}

public class InteractManager : MonoBehaviour
{
	public static InteractManager Instance;

	public Canvas canvas;

	public GameObject interactPromptPrefab;

	private class Interactible
	{
		public IInteractible interactible;
		public Vector2 position;

		public InteractPrompt prompt;

		public Interactible(IInteractible interactible, Vector2 position)
		{
			this.interactible = interactible;
			this.position = position;
		}
	}
	private List<Interactible> interactibles = new List<Interactible>();

	private Interactible currentInteractible;

	private Transform player;
	private PlayerActions playerActions;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if(GameManager.instance && GameManager.instance.player)
		{
			player = GameManager.instance.player.transform;
		}

		playerActions = ControlManager.GetPlayerActions();
	}

	private void Update()
	{
		if (player)
		{
			Interactible closestInteractible = null;
			float closestDistance = float.MaxValue;

			foreach(Interactible interact in interactibles)
			{
				float distance = Vector3.Distance(interact.position, player.position);
				if(distance < closestDistance)
				{
					closestInteractible = interact;
					closestDistance = distance;
				}
			}

			//If a new interactible is chosen
			if(closestInteractible != null && closestInteractible != currentInteractible)
			{
				//Hide previous prompt
				if(currentInteractible != null && currentInteractible.prompt != null)
				{
					currentInteractible.prompt.HidePrompt();
					currentInteractible.prompt = null;
				}

				//Set current
				currentInteractible = closestInteractible;

				//Show prompt
				if(interactPromptPrefab)
				{
					GameObject obj = ObjectPooler.GetPooledObject(interactPromptPrefab);
					obj.transform.SetParent(canvas.transform);

					obj.transform.localScale = interactPromptPrefab.transform.localScale;
					obj.transform.localPosition = Vector3.zero;

					//Make sure to render behind other canvas elements
					obj.transform.SetAsFirstSibling();

					KeepWorldPosOnCanvas keep = obj.GetComponent<KeepWorldPosOnCanvas>();
					if (keep)
						keep.worldPos = currentInteractible.position;

					InteractPrompt interactPrompt = obj.GetComponent<InteractPrompt>();
					if(interactPrompt)
					{
						currentInteractible.prompt = interactPrompt;
						interactPrompt.ShowPrompt(currentInteractible.interactible.InteractType);
					}
				}
			}
		}

		if(currentInteractible != null && playerActions.Interact.WasPressed && GameManager.instance.CanDoActions)
		{
			currentInteractible.interactible.Interact();
		}
	}

	public static void AddInteractible(IInteractible interactible, Vector2 position)
	{
		if (Instance == null)
			return;

		//If already in list just update position and return
		foreach(Interactible interact in Instance.interactibles)
		{
			if(interact.interactible == interactible)
			{
				interact.position = position;
				return;
			}
		}

		//else, add to list
		Instance.interactibles.Add(new Interactible(interactible, position));
	}

	public static void RemoveInteractible(IInteractible interactible)
	{
		if (Instance == null)
			return;

		Interactible interactiblePosition = null;

		foreach(Interactible interact in Instance.interactibles)
		{
			if (interact.interactible == interactible)
				interactiblePosition = interact;
		}

		if(interactiblePosition != null)
		{
			Instance.interactibles.Remove(interactiblePosition);

			if(Instance.currentInteractible == interactiblePosition)
			{
				Instance.currentInteractible.prompt.HidePrompt();
				Instance.currentInteractible = null;
			}
		}
	}
}
