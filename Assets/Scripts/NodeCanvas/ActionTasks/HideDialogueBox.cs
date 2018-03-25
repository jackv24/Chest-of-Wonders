using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Dialogue")]
	public class HideDialogueBox : ActionTask
	{
		protected override void OnExecute()
		{
			if(DialogueBox.Instance && DialogueBox.Instance.IsDialogueOpen)
			{
				DialogueBox.Instance.HideDialogueBox();
			}

			EndAction(true);
		}
	}
}
