using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Name("Curve Tween (Additive)")]
	[Category("Movement/Direct")]
	[Description("Custom curve tween that ignores and 0 axis, allowing stacking of multiple axis tweens.")]
	public class CurveTransformTweenAdditive : ActionTask<Transform>
	{
		public BBParameter<Vector3> targetPosition;
		public BBParameter<AnimationCurve> curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		public BBParameter<float> time = 0.5f;

		private Vector3 initialPosition;
		private Vector3 finalPosition;

		protected override void OnExecute()
		{
			initialPosition = agent.localPosition;

			finalPosition = targetPosition.value + initialPosition;

			if ((initialPosition - finalPosition).magnitude < 0.1f)
				EndAction();
		}

		protected override void OnUpdate()
		{
			var value = Vector3.Lerp(initialPosition, finalPosition, curve.value.Evaluate(elapsedTime / time.value));

			if (targetPosition.value.x != 0)
				agent.SetLocalPositionX(value.x);
			if (targetPosition.value.y != 0)
				agent.SetLocalPositionY(value.y);
			if (targetPosition.value.z != 0)
				agent.SetLocalPositionZ(value.z);

			if (elapsedTime >= time.value)
				EndAction(true);
		}
	}
}
