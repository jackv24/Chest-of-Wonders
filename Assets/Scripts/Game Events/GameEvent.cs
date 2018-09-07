using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/Game Event")]
public class GameEvent : ScriptableObject
{
	[NonSerialized]
	private List<GameEventListener> eventListeners;

	public void Raise()
	{
		int listenerCountBefore = 0;
		int listenerCountAfter = 0;

		if (eventListeners != null)
		{
			listenerCountBefore = eventListeners.Count;

			// Loop backwards through list in case listeners unsubscribe when called
			for (int i = 0; i < eventListeners.Count; i++)
				eventListeners[i].OnEventRaised();

			listenerCountAfter = eventListeners.Count;
		}

		Debug.Log($"GameEvent \"{name}\" raised. Listeners before: {listenerCountBefore}, after: {listenerCountAfter}");
	}

	public void RegisterListener(GameEventListener listener)
	{
		if (eventListeners == null)
			eventListeners = new List<GameEventListener>();

		eventListeners.Add(listener);
	}

	public void DeregisterListener(GameEventListener listener)
	{
		if (eventListeners == null)
			return;

		eventListeners.Remove(listener);
	}
}

public static class GameEventExtensions
{
	public static void RaiseSafe(this GameEvent gameEvent)
	{
		if (gameEvent)
			gameEvent.Raise();
	}
}
