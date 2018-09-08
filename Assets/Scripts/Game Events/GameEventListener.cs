using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[SerializeField]
	private GameEvent listenTo;

	[Space(), SerializeField]
	private UnityEvent response;

	private void OnEnable()
	{
		if (listenTo)
			listenTo.RegisterListener(this);
	}

	private void OnDisable()
	{
		if (listenTo)
			listenTo.DeregisterListener(this);
	}

	public void SetEvent(GameEvent gameEvent)
	{
		if (listenTo)
			listenTo.DeregisterListener(this);

		if(gameEvent)
		{
			listenTo = gameEvent;
			listenTo.RegisterListener(this);
		}
	}

	public void OnEventRaised()
	{
		response.Invoke();
	}

	public void AddResponse(UnityAction call)
	{
		if (response == null)
			response = new UnityEvent();
		response.AddListener(call);
	}

	public void RemoveResponse(UnityAction call)
	{
		if(response != null)
			response.RemoveListener(call);
	}
}
