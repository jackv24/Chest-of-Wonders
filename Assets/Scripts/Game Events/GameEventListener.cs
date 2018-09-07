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

	public void OnEventRaised()
	{
		response.Invoke();
	}
}
