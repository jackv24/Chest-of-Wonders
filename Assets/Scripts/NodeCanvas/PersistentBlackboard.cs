using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class PersistentBlackboard : MonoBehaviour
{
	public string key;

	private Blackboard blackboard;

	private void Awake()
	{
		blackboard = GetComponent<Blackboard>();
	}

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
		if (!blackboard || string.IsNullOrEmpty(key))
			return;

		if(SaveManager.instance)
		{
			SaveManager.instance.AddTempLoadAction((data) =>
			{
				string json;
				if(data.GetBlackboardJson(key, out json))
				{
					blackboard.Deserialize(json);
				}
			});

			SaveManager.instance.AddTempSaveAction((data, hardSave) =>
			{
				data.SaveBlackBoardJson(key, blackboard.Serialize());
			});
		}
	}
}
