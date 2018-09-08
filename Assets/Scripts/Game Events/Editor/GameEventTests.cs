using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;

public class GameEventTests
{
	[Test]
	public void EventRaisedListenerResponds()
	{
		var gameEvent = ScriptableObject.CreateInstance<GameEvent>();
		var eventListener = new GameObject("Listener", typeof(GameEventListener)).GetComponent<GameEventListener>();

		eventListener.SetEvent(gameEvent);

		bool responded = false;
		eventListener.AddResponse(() => responded = true);

		gameEvent.Raise();

		Assert.AreEqual(responded, true);
	}

	[Test]
	public void EventRaisedListenerRespondsMultiple()
	{
		var gameEvent = ScriptableObject.CreateInstance<GameEvent>();

		var responded = new bool[5];
		for (int i = 0; i < responded.Length; i++)
		{
			var eventListener = new GameObject($"Listener {i}", typeof(GameEventListener)).GetComponent<GameEventListener>();

			eventListener.SetEvent(gameEvent);

			// Cache index to ensure fixed value
			int index = i;
			eventListener.AddResponse(() => responded[index] = true);
		}

		gameEvent.Raise();

		Assert.AreEqual(responded.Where(r => r == true).ToArray().Length, responded.Length);
	}
}
