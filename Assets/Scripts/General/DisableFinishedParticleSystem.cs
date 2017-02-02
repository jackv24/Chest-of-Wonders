using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFinishedParticleSystem : MonoBehaviour
{
    //The particle system to get duration from
    private ParticleSystem system;

    void Awake()
    {
        //Get particle system (may be a component of a child gameobject)
        system = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        //Disable the gameobject (for object pooling) after the particle system has finished
        StartCoroutine("DisableAfterTime", system.main.duration);
    }

    IEnumerator DisableAfterTime(float time)
    {
        //Wait for time
        yield return new WaitForSeconds(time);

        //Disable (return to pool)
        gameObject.SetActive(false);
    }
}
