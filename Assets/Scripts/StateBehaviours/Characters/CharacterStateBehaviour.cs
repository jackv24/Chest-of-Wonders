using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for StateMachineBehaviours that are intended to be used with character animations
/// </summary>
public abstract class CharacterStateBehaviour : StateMachineBehaviour
{
	public enum GetLevel
	{
		Self, Parents, Children
	}

	//Character graphic is usually a child of the character
	public GetLevel getLevel = GetLevel.Parents;

	//Cache GetComponents to improve speed on subsequent entries of this state
	private Dictionary<System.Type, Component> cachedComponents = new Dictionary<System.Type, Component>();

	//Behaviour can end before end of state
	[Range(0, 1.0f)]
	public float endTime = 1.0f;
	protected bool hasEnded;

	protected float normalizedTime;

	protected CharacterStats characterStats;
	protected PlayerInput playerInput; //A character can be controlled by the player, but doesn't need to be

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		characterStats = GetComponentAtLevel<CharacterStats>(animator.gameObject);
		playerInput = GetComponentAtLevel<PlayerInput>(animator.gameObject);

		if (characterStats)
			characterStats.damageImmunity = true;

		if (playerInput)
			playerInput.AcceptingInput = PlayerInput.InputAcceptance.None;

		hasEnded = false;

		normalizedTime = 0;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= endTime)
			EndBehaviour();

		//Remap state normalised time to accoutn for behaviour ending early
		normalizedTime = Mathf.Clamp(stateInfo.normalizedTime / endTime, 0, 1.0f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EndBehaviour();
	}

	protected virtual void EndBehaviour()
	{
		if (hasEnded)
			return;
		hasEnded = true;

		if (characterStats)
			characterStats.damageImmunity = false;

		if (playerInput)
			playerInput.AcceptingInput = PlayerInput.InputAcceptance.All;
	}

	/// <summary>
	/// Gets a component from on or around a GameObject (useful since a Character setup usually has the animator in a child).
	/// </summary>
	/// <typeparam name="T">The type of component to get.</typeparam>
	/// <param name="gameObject">The target gameobject (usually animator.gameObject)</param>
	protected T GetComponentAtLevel<T>(GameObject gameObject) where T : Component
	{
		T component = null;
		System.Type type = typeof(T);

		//No need to find the component if we've already found one of the same type
		// (there should not be more than 1 of any type of character component on a character)
		if (cachedComponents.ContainsKey(type))
			return (T)cachedComponents[type];
		else
		{
			//Get collider from wherever specified
			switch (getLevel)
			{
				case GetLevel.Self:
					component = gameObject.GetComponent<T>();
					break;
				case GetLevel.Parents:
					component = gameObject.GetComponentInParent<T>();
					break;
				case GetLevel.Children:
					component = gameObject.GetComponentInChildren<T>();
					break;
			}

			//Cache this component to speed up subsequent calls
			cachedComponents[typeof(T)] = component;

			return component;
		}
	}
}
