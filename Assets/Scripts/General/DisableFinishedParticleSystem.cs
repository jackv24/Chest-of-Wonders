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
			StartCoroutine(DisableAfterTime(system.main.duration));
		}
		else
		{
			StartCoroutine(DisableAfterParticles());
		}
    }

    IEnumerator DisableAfterTime(float time)
    {
		yield return new WaitForSeconds(checkDelay);

        //Wait for time
        yield return new WaitForSeconds(time);

        //Disable (return to pool)
        gameObject.SetActive(false);
    }

	IEnumerator DisableAfterParticles()
	{
		yield return new WaitForSeconds(checkDelay);

		while (system.isPlaying)
			yield return new WaitForEndOfFrame();

		gameObject.SetActive(false);
	}
}
