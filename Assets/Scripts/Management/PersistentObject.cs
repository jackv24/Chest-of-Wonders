using UnityEngine;
using System.Collections;

[System.Serializable]
public class PersistentObject
{
	public string id = string.Empty;
	public string sceneName = string.Empty;

	[Space()]
	public bool activated = false;

	public void GetID(GameObject gameObject)
	{
		id = gameObject.name;

		sceneName = gameObject.scene.name;
	}

	public void SaveState(bool activated)
	{
		this.activated = activated;

		if(!string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(id))
			SaveManager.instance.SetPersistentObjectState(sceneName, id, activated);
	}

	public void LoadState(ref bool activated)
	{
		if (!string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(id))
			this.activated = SaveManager.instance.GetPersistentObjectState(sceneName, id);

		activated = this.activated;
	}
}
