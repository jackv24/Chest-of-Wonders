﻿using System.Collections;
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
	private static InteractManager instance;

	public static bool CanInteract
	{
		get { return instance && instance.canInteract; }
		set
		{
			if (instance)
				instance.canInteract = value;
		}
	}

	private bool canInteract = true;

	public Canvas canvas;

	public GameObject interactPromptPrefab;

	public float interactDelay = 0.5f;
	private float nextInteractTime = 0;

	private class Interactible
	{
		public IInteractible interactible;
		public Vector2 position;
		public Vector2 promptOffset;

		public InteractPrompt prompt;

		public Interactible(IInteractible interactible, Vector2 position, Vector2 promptOffset)
		{
			this.interactible = interactible;
			this.position = position;
			this.promptOffset = promptOffset;
		}
	}
	private List<Interactible> interactibles = new List<Interactible>();

	private Interactible currentInteractible;

	private Transform player;
	private PlayerActions playerActions;

	private void Awake()
	{
		instance = this;
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
					currentInteractible = null;
				}

				//Only show interact prompt if they can be interacted with at this time
				if (canInteract && GameManager.instance.CanDoActions && Time.time >= nextInteractTime)
				{
					//Set current
					currentInteractible = closestInteractible;

					//Show prompt
					if (interactPromptPrefab)
					{
						GameObject obj = ObjectPooler.GetPooledObject(interactPromptPrefab);
						obj.transform.SetParent(canvas.transform);

						obj.transform.localScale = interactPromptPrefab.transform.localScale;
						obj.transform.localPosition = Vector3.zero;

						//Make sure to render behind other canvas elements
						obj.transform.SetAsFirstSibling();

						KeepWorldPosOnCanvas keep = obj.GetComponent<KeepWorldPosOnCanvas>();
						if (keep)
							keep.worldPos = currentInteractible.position + currentInteractible.promptOffset;

						InteractPrompt interactPrompt = obj.GetComponent<InteractPrompt>();
						if (interactPrompt)
						{
							currentInteractible.prompt = interactPrompt;
							interactPrompt.ShowPrompt(currentInteractible.interactible.InteractType);
						}
					}
				}
			}
		}

		if(canInteract && currentInteractible != null && playerActions.Interact.WasPressed && GameManager.instance.CanDoActions)
		{
			nextInteractTime = Time.time + interactDelay;

			currentInteractible.interactible.Interact();
		}
	}

	public static void AddInteractible(IInteractible interactible, Vector2 position, Vector2 promptOffset)
	{
		if (instance == null)
			return;

		//If already in list just update position and return
		foreach(Interactible interact in instance.interactibles)
		{
			if(interact.interactible == interactible)
			{
				interact.position = position;
				return;
			}
		}

		//else, add to list
		instance.interactibles.Add(new Interactible(interactible, position, promptOffset));
	}

	public static void RemoveInteractible(IInteractible interactible)
	{
		if (instance == null)
			return;

		Interactible interactiblePosition = null;

		foreach(Interactible interact in instance.interactibles)
		{
			if (interact.interactible == interactible)
				interactiblePosition = interact;
		}

		if(interactiblePosition != null)
		{
			instance.interactibles.Remove(interactiblePosition);

			if(instance.currentInteractible == interactiblePosition)
			{
				instance.currentInteractible.prompt.HidePrompt();
				instance.currentInteractible = null;
			}
		}
	}
}
