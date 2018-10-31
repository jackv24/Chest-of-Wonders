using System.Collections.Generic;
using UnityEngine;

public abstract class CachedStateBehaviour : StateMachineBehaviour
{
    public enum GetLevel
    {
        Self, Parents, Children
    }

    //Character graphic is usually a child of the character
    public GetLevel getLevel = GetLevel.Parents;

    //Cache GetComponents to improve speed on subsequent entries of this state
    private Dictionary<System.Type, Component> cachedComponents = new Dictionary<System.Type, Component>();

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
