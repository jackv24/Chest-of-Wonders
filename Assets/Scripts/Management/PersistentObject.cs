using UnityEngine;
using System.Collections;

[System.Serializable]
public class PersistentObject
{
	private readonly SaveManager saveManager;

	public PersistentObject()
	{
		saveManager = SaveManager.instance;
	}

	public PersistentObject(SaveManager saveManager)
	{
		this.saveManager = saveManager;
	}

	public delegate void StateLoadedEvent(bool activated);
	public event StateLoadedEvent OnStateLoaded;

	[SerializeField]
	private string id = string.Empty;

	[SerializeField]
	private string sceneName = string.Empty;

	[Space(), SerializeField]
	private bool activated = false;

	/// <summary>
	/// Should be called after OnStateLoaded in subscribed.
	/// </summary>
	/// <param name="gameObject">The owning gameobject from which to get scene and name. Assumes there is only one PersistentObject per GameObject.</param>
	public void Setup(GameObject gameObject)
	{
		if(string.IsNullOrEmpty(id))
			id = gameObject.name;

		if(string.IsNullOrEmpty(sceneName))
			sceneName = gameObject.scene.name;

		if(saveManager)
		{
			saveManager.AddTempLoadAction((data) =>
			{
				activated = data.GetPersistentObjectState(sceneName, id);

				OnStateLoaded?.Invoke(activated);
			});

			saveManager.AddTempSaveAction((data, hardSave) =>
			{
				saveManager.SetPersistentObjectState(sceneName, id, activated);
			});
		}
	}

	public void SaveState(bool activated)
	{
		this.activated = activated;
	}
}
