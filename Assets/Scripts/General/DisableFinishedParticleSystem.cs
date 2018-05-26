using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFinishedParticleSystem : MonoBehaviour
{
    //The particle system to get duration from
    private ParticleSystem system;

	public enum WaitType { Time, Particles }
	public WaitType waitType = WaitType.Time;

	public float checkDelay = 0.0f;

	private float delayTime;

    void Awake()
    {
		//Get particle system (may be a component of a child gameobject)
		system = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
		if (waitType == WaitType.Time)
		{
			//Disable the gameobject (for object pooling) after the particle system has finished
			delayTime = Time.time + system.main.duration + checkDelay;
		}
		else
		{
			delayTime = Time.time + checkDelay;
		}
    }

	private void Update()
	{
		if(waitType == WaitType.Particles)
		{
			if(!system.IsAlive(true) && Time.time >= delayTime)
				gameObject.SetActive(false);
		}
		else if(waitType == WaitType.Time)
		{
			if (Time.time >= delayTime)
				gameObject.SetActive(false);
		}
	}
}
