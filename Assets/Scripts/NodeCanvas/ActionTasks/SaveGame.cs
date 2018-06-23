using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("System")]
	public class SaveGame : ActionTask
	{
		public BBParameter<string> spawnMarkerName = new BBParameter<string>("SpawnMarker_NPC");
		public BBParameter<bool> hardSave = new BBParameter<bool>(true);

		protected override void OnExecute()
		{
			if (GameManager.instance)
			{
				GameManager.instance.LastSaveLocation = new SaveData.Location(GameManager.instance.LoadedSceneName, spawnMarkerName.value);
			}

			SaveManager.instance?.SaveGame(hardSave.value);

			EndAction(true);
		}
	}
}
