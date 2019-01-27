using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(ParticleSystem))]
public class TrailParticles : MonoBehaviour
{
    [SerializeField]
	private float stopGroundedDelay = 0.1f;
	private Coroutine stopParticlesRoutine = null;

	private bool wasGrounded;
    private Func<bool> groundedFunction;

	private ParticleSystem system;
	private DisableFinishedParticleSystem disableFinished;

	private Transform previousParent;

	private void Awake()
	{
		system = GetComponent<ParticleSystem>();

		disableFinished = GetComponent<DisableFinishedParticleSystem>();
	}

	private void Update()
	{
		if(groundedFunction != null)
		{
            bool isGrounded = groundedFunction();

            // Update particle emission state when grounded state changes
            if (isGrounded != wasGrounded)
			{
				wasGrounded = isGrounded;

				// Play particles only when on ground
				if (wasGrounded)
				{
					if (stopParticlesRoutine != null)
						StopCoroutine(stopParticlesRoutine);

					system.Play(true);
				}
				else
				{
					stopParticlesRoutine = StartCoroutine(StopParticlesDelayed());
				}
			}
		}
	}

	private IEnumerator StopParticlesDelayed()
	{
		yield return new WaitForSeconds(stopGroundedDelay);

		system.Stop(true, ParticleSystemStopBehavior.StopEmitting);

		stopParticlesRoutine = null;
	}

	/// <summary>
	/// Sets up and starts the emission of the trail particles.
	/// </summary>
	/// <param name="target">The target to follow (will be temporarily set as a child).</param>
	/// <param name="characterMove">The CharacterMove to get IsGrounded from.</param>
	public void StartParticles(Transform target, Func<bool> groundedFunction)
	{
		this.groundedFunction = groundedFunction;

		//Start of not emitting particles until we are sure we're grounded
		wasGrounded = false;

		//Don't disable the particle system when it's not emitting yet
		disableFinished.enabled = false;

		//Set parent to target for easy follow and inherit velocity support
		previousParent = transform.parent;
		transform.SetParent(target, true);
	}

	/// <summary>
	/// Stops emmitting the particles and allows it to be recycled
	/// </summary>
	public void StopParticles()
	{
		//Set parent back to previous (should be object pool parent)
		transform.SetParent(previousParent);
		previousParent = null;
        groundedFunction = null;

		//Allow the particle system to disable itself
		disableFinished.enabled = true;

		//Stop emitting particles (will recycle itself when there are no live particles left)
		if (stopParticlesRoutine != null)
			StopCoroutine(stopParticlesRoutine);
		stopParticlesRoutine = StartCoroutine(StopParticlesDelayed());
	}
}
