using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using System.Linq;

public class PersistentBlackboard : MonoBehaviour
{
	public string key;

	private Blackboard blackboard;

	private void Reset()
	{
		key = gameObject.name;

		//We want to destroy global blackboards since we handle persistence with save/load
		GlobalBlackboard globalBlackboard = GetComponent<GlobalBlackboard>();
		if (globalBlackboard)
			globalBlackboard.dontDestroy = false;
	}

	private void Start()
	{
		Setup(SaveManager.instance);
	}

	public void Setup(SaveManager saveManager)
	{
		blackboard = GetComponent<Blackboard>();

		if (!blackboard || string.IsNullOrEmpty(key))
			return;

		if (saveManager)
		{
			saveManager.AddTempLoadAction((data) =>
			{
				string json;
				if (data.GetBlackboardJson(key, out json))
				{
					var beforeVariables = new Dictionary<string, Variable>(blackboard.variables);
					blackboard.Deserialize(json);

					// Merge existing variables with deserialized variables (prefer deserialized)
					blackboard.variables = blackboard.variables
					.Concat(beforeVariables
					.Where(kvp => !blackboard.variables.ContainsKey(kvp.Key)))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				}
			});

			saveManager.AddTempSaveAction((data, hardSave) =>
			{
				data.SaveBlackBoardJson(key, blackboard.Serialize());
			});
		}
	}
}
