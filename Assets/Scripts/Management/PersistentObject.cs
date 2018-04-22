using UnityEngine;
using System.Collections;

[System.Serializable]
public class PersistentObject
{
	public delegate void StateLoadedEvent(bool activated);
	public event StateLoadedEvent OnStateLoaded;

	public string id = string.Empty;
	public string sceneName = string.Empty;

	[Space()]
	public bool activated = false;

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

		if(SaveManager.instance)
		{
			if (SaveManager.instance.IsDataLoaded)
				LoadState();
			else
			{
				SaveManager.DataLoadedEvent temp = null;
				temp = (SaveData data) =>
				{
					LoadState();

					SaveManager.instance.OnDataLoaded -= temp;
				};
				SaveManager.instance.OnDataLoaded += temp;
			}
		}
	}

	public void SaveState(bool activated)
	{
		this.activated = activated;

		if(!string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(id))
			SaveManager.instance.SetPersistentObjectState(sceneName, id, activated);
	}

	private void LoadState()
	{
		if (!string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(id))
			activated = SaveManager.instance.GetPersistentObjectState(sceneName, id);

		OnStateLoaded?.Invoke(activated);
	}
}
