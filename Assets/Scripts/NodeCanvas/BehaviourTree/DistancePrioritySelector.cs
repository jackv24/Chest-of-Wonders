using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Category("Composites")]
	[Description("Used for Utility AI, the Distance Priority Selector executes the child with the closest value above the target distance. If it fails, the Distance Priority Selector will continue with the next furthest distance child until one Succeeds, or until all Fail (similar to how a normal Selector does).")]
	[Icon("DistancePriority")]
	[Color("99ccff")]
	[Name("Distance Priority Selector (Custom)")]
	public class DistancePrioritySelector : BTComposite
	{
		public BBParameter<GameObject> target;

		public List<BBParameter<float>> priorities = new List<BBParameter<float>>();

		private List<Connection> orderedConnections = new List<Connection>();
		private Connection currentConnection = null;

		public override string name
		{
			get { return base.name.ToUpper(); }
		}

		public override void OnChildConnected(int index)
		{
			priorities.Insert(index, new BBParameter<float> { value = 1, bb = graphBlackboard });
		}

		public override void OnChildDisconnected(int index)
		{
			priorities.RemoveAt(index);
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			//Will only work if there is a target
			if (target.isNone)
				return Status.Failure;

			float distance = Vector2.Distance(agent.transform.position, target.value.transform.position);

			if (status == Status.Resting)
			{
				//Check furthest distances first
				orderedConnections = outConnections.OrderBy(c => priorities[outConnections.IndexOf(c)].value).ToList();
			}

			//If there is a running connection, execute that, and continue if it fails
			if(currentConnection != null)
			{
				status = currentConnection.Execute(agent, blackboard);

				if (status == Status.Success)
					return Status.Success;

				if (status == Status.Running)
					return Status.Running;
			}

			//If there was a running connection, start from after it, otherwise start from zero
			for (var i = (currentConnection == null ? 0 : (orderedConnections.IndexOf(currentConnection) + 1)); i < orderedConnections.Count; i++)
			{
				float d = priorities[outConnections.IndexOf(orderedConnections[i])].value;

				//Attempt next connection if this once is too far
				if (distance > d)
					continue;

				status = orderedConnections[i].Execute(agent, blackboard);
				if (status == Status.Success)
				{
					return Status.Success;
				}

				if (status == Status.Running)
				{
					currentConnection = orderedConnections[i];
					return Status.Running;
				}
			}

			return Status.Failure;
		}

		protected override void OnReset()
		{
			currentConnection = null;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
#if UNITY_EDITOR

		public override string GetConnectionInfo(int i)
		{
			return priorities[i].ToString();
		}

		public override void OnConnectionInspectorGUI(int i)
		{
			priorities[i] = (BBParameter<float>)EditorUtils.BBParameterField("Distance", priorities[i]);
		}

		protected override void OnNodeInspectorGUI()
		{
			target = (BBParameter<GameObject>)EditorUtils.BBParameterField("Distance Target", target);

			for (var i = 0; i < priorities.Count; i++)
				priorities[i] = (BBParameter<float>)EditorUtils.BBParameterField("Distance", priorities[i]);
		}
#endif

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();

			for (var i = 0; i < priorities.Count; i++)
			{
				Gizmos.color = new Color(1, 0, 0, 0.1f);
				Gizmos.DrawSphere(graphAgent.transform.position, priorities[i].value);
				Gizmos.color = new Color(1, 0, 0, 1);
				Gizmos.DrawWireSphere(graphAgent.transform.position, priorities[i].value);
			}
		}
	}
}
