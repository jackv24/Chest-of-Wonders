using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a sound when a particle collision occurs. Must be attached to the same GameObject as the ParticleSystem.
/// </summary>
public class ParticleCollisionSounds : MonoBehaviour
{
	public SoundType soundType;
	public SoundEventRandom collisionSounds;

	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

	private ParticleSystem system;

	private void Awake()
	{
		system = GetComponent<ParticleSystem>();
	}

	private void OnParticleCollision(GameObject other)
	{
		int eventCount = system.GetCollisionEvents(other, collisionEvents);

		for(int i = 0; i < eventCount; i++)
		{
			collisionSounds.Play(collisionEvents[i].intersection, soundType);
		}
	}
}
