using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassReact : MonoBehaviour, IDamageable
{
	public SpriteRenderer[] grassPieces;

	[System.Serializable]
	public struct PushProperties
	{
		public AnimationCurve curve;
		public float duration;
		public float multiplier;
	}

	public PushProperties pushX = new PushProperties
	{
		curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0)),
		duration = 0.5f,
		multiplier = 1.0f
	};
	public PushProperties pushY = new PushProperties
	{
		curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0)),
		duration = 0.5f,
		multiplier = 1.0f
	};

	private const float velocityThreshold = 0.5f;

	[System.Serializable]
	public struct ColorFade
	{
		public Gradient gradient;
		public float duration;
	}

	public ColorFade burnFade;
	private Coroutine burnRoutine = null;

	private Coroutine pushRoutineX = null;
	private Coroutine pushRoutineY = null;
	private MaterialPropertyBlock propBlock;

	private Transform target;
	private Vector2 lastPosition;
	private bool waitedFrame;

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	private void Update()
	{
		if(target)
		{
			if (!target.gameObject.activeInHierarchy)
			{
				target = null;
			}
			else
			{
				//Wait one frame to determine the velocity of the target
				if (waitedFrame)
				{
					//Calculate velocity and then store current position as last position for next frame
					Vector2 velocity = ((Vector2)target.position - lastPosition) / Time.deltaTime; //Divide by delta time to change from per frame to per second
					lastPosition = target.position;

					if (velocity.magnitude >= velocityThreshold)
					{
						DoPush(velocity.normalized);
					}
				}
				else
					waitedFrame = true;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(!target)
		{
			target = collision.transform;
			lastPosition = target.position;
			waitedFrame = false;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		target = null;
	}

	/// <summary>
	/// Reusable axis-agnostic push routine. Just handles evaulation of push curve, actual pushing is handled by delegate.
	/// </summary>
	private IEnumerator PushRoutine(PushProperties push, System.Action<float> onEvaluateCurve, System.Action onFinish)
	{
		float elapsed = 0;
		while(elapsed <= push.duration)
		{
			onEvaluateCurve?.Invoke(push.curve.Evaluate(elapsed / push.duration));

			yield return null;
			elapsed += Time.deltaTime;
		}

		onEvaluateCurve?.Invoke(0);

		onFinish?.Invoke();
	}

	private void SetGrassPushAmount(float? x, float? y)
	{
		//Set grass push property this way since it uses [PerRendererData] in the shader
		foreach(var piece in grassPieces)
		{
			piece.GetPropertyBlock(propBlock);

			if(x != null)
				propBlock.SetFloat("_PushAmountX", x.Value);
			if(y != null)
				propBlock.SetFloat("_PushAmountY", y.Value);

			piece.SetPropertyBlock(propBlock);
		}
	}

	private void SetGrassColor(Color color)
	{
		foreach (var piece in grassPieces)
			piece.color = color;
	}

	private void DoPush(Vector2 direction)
	{
		float pushAmountX = pushX.multiplier * direction.x;
		float pushAmountY = pushY.multiplier * direction.y;

		if (pushRoutineX == null)
			pushRoutineX = StartCoroutine(PushRoutine(pushX, (value) => { SetGrassPushAmount(value * pushAmountX, null); }, () => { pushRoutineX = null; }));

		if (pushRoutineY == null)
			pushRoutineY = StartCoroutine(PushRoutine(pushY, (value) => { SetGrassPushAmount(null, value * pushAmountY); }, () => { pushRoutineY = null; }));
	}

	public bool TakeDamage(DamageProperties damageProperties)
	{
		//Only react to damage from melee attacks, etc
		if(damageProperties.type == DamageType.Regular)
		{
			DoPush(damageProperties.direction);

			//If the attack was a fire type then also start burning
			if(damageProperties.sourceElement == ElementManager.Element.Fire)
			{
				if (burnRoutine == null)
					burnRoutine = StartCoroutine(BurnRoutine());
			}
		}

		//Always return false since we don't want any hit effects
		return false;
	}

	private IEnumerator BurnRoutine()
	{
		float elapsed = 0;
		while (elapsed <= burnFade.duration)
		{
			SetGrassColor(burnFade.gradient.Evaluate(elapsed / burnFade.duration));

			yield return null;
			elapsed += Time.deltaTime;
		}

		SetGrassColor(burnFade.gradient.Evaluate(1.0f));

		//Don't clear burnRoutine reference since we only want the burn to happen once
	}
}
