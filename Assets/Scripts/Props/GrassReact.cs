using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassReact : MonoBehaviour
{
	public SpriteRenderer[] grassPieces;

	public AnimationCurve pushCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
	public float pushDuration = 0.5f;
	public float pushMultiplier = 1.0f;

	private Coroutine pushRoutine = null;
	private MaterialPropertyBlock propBlock;

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Only push again if we're not in the middle of a push
		if(pushRoutine == null)
		{
			Vector2 direction = collision.transform.position - transform.position;
			direction.y = 0;
			direction.Normalize();

			pushRoutine = StartCoroutine(PushRoutine(-direction.x));
		}
	}

	private IEnumerator PushRoutine(float direction)
	{
		float elapsed = 0;
		while(elapsed <= pushDuration)
		{
			SetGrassPushAmount(pushCurve.Evaluate(elapsed / pushDuration) * direction * pushMultiplier);

			yield return null;
			elapsed += Time.deltaTime;
		}

		SetGrassPushAmount(0);

		pushRoutine = null;
	}

	private void SetGrassPushAmount(float pushAmount)
	{
		//Set grass push property this way since it uses [PerRendererData] in the shader
		foreach(var piece in grassPieces)
		{
			piece.GetPropertyBlock(propBlock);

			propBlock.SetFloat("_PushAmount", pushAmount);

			piece.SetPropertyBlock(propBlock);
		}
	}
}
