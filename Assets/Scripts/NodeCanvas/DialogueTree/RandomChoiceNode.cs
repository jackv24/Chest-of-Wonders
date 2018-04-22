using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.DialogueTrees{

	[Icon("RandomChoice")]
	[Name("Random Choice")]
	[Category("Branch")]
	[Description("Will randomly choose a child node.")]
	[Color("b3ff7f")]
	public class RandomChoiceNode : DTNode
	{
		private struct Pair
		{
			public float weight;
			public int index;
		}

		[SerializeField]
		private List<BBParameter<float>> weights = new List<BBParameter<float>>();

		public override int maxOutConnections
		{
			get { return -1; }
		}

		public override void OnChildConnected(int connectionIndex)
		{
			weights.Insert(connectionIndex, new BBParameter<float>(1.0f));
		}

		public override void OnChildDisconnected(int connectionIndex)
		{
			weights.RemoveAt(connectionIndex);
		}

		protected override Status OnExecute(Component agent, IBlackboard bb)
		{
			if (outConnections.Count == 0)
				return Error("There are no connections on the Dialogue Condition Node");

			//Create sorted list of index/weight pairs
			List<Pair> pairs = new List<Pair>(weights.Count);
			for (int i = 0; i < weights.Count; i++)
				pairs.Add(new Pair { index = i, weight = weights[i].value });
			pairs.Sort((a, b) => { return a.weight.CompareTo(b.weight); });

			//Add all weights together to get max probability
			float maxProbability = 0;
			foreach(Pair pair in pairs)
				maxProbability += pair.weight;

			//Generate random number up to max
			float num = Random.Range(0, maxProbability);

			//Get random index using cumulative probability
			float runningProbability = 0;
			Pair chosenPair = new Pair(); //Should never not be reassigned, but needs to be assigned here otherwise compiler complains
			foreach(Pair pair in pairs)
			{
				if (num >= runningProbability)
					chosenPair = pair;

				runningProbability += pair.weight;
			}

			DLGTree.Continue(chosenPair.index);
			return Status.Success;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
#if UNITY_EDITOR

		public override void OnConnectionInspectorGUI(int i)
		{
			EditorUtils.BBParameterField("Weight", weights[i]);
		}

		public override string GetConnectionInfo(int i)
		{
			float total = GetTotal();

			return ((weights[i].value / total) * 100) + "%";
		}

		protected override void OnNodeInspectorGUI()
		{
			if (outConnections.Count == 0)
			{
				GUILayout.Label("Make some connections first");
				return;
			}

			float total = GetTotal();
			for (var i = 0; i < weights.Count; i++)
			{
				GUILayout.BeginHorizontal();
				weights[i] = (BBParameter<float>)EditorUtils.BBParameterField("Weight", weights[i]);
				GUILayout.Label(Mathf.Round((weights[i].value / total) * 100) + "%", GUILayout.Width(38));
				GUILayout.EndHorizontal();
			}
		}

		protected override void OnNodeGUI()
		{
			if (outConnections.Count == 0){
				GUILayout.Label("No Outcomes Connected");
			}
		}

		private float GetTotal()
		{
			float total = 0;

			foreach (var weight in weights)
				total += weight.value;

			return total;
		}
#endif
	}
}
