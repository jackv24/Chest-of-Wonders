using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAfterImageEffect : MonoBehaviour
{
	public int preSpawnAmount = 5;

	public float spawnDistance = 0.1f;
	public float lifeTime = 0.1f;

	public Gradient fadeOutGradient;

	private Vector2? lastSpawnPos;
	private bool doSpawn = false;

	public Material material;

	private GameObject template;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		template = new GameObject(string.Format("AfterImage"));
		template.transform.SetParent(transform, false);

		SpriteRenderer renderer = template.AddComponent<SpriteRenderer>();
		renderer.material = material;

		//Setup pool before use for performance
		ObjectPooler.SetupPool(template, Mathf.CeilToInt(preSpawnAmount));

		template.SetActive(false);
	}

	private void Update()
	{
		if(doSpawn)
		{
			//Spawn an image after moving a certain distance
			if(lastSpawnPos == null || Vector2.Distance(lastSpawnPos.Value, transform.position) >= spawnDistance)
			{
				lastSpawnPos = transform.position;

				GameObject obj = ObjectPooler.GetPooledObject(template);
				obj.transform.position = transform.position;

				obj.transform.localScale = transform.localScale;

				//Set images sprite to match current sprite
				SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
				renderer.sprite = spriteRenderer.sprite;

				StartCoroutine(FadeOutImage(renderer));
			}
		}


	}

	public void StartAfterImageEffect()
	{
		doSpawn = true;
		lastSpawnPos = null;
	}

	public void EndAfterImageEffect()
	{
		doSpawn = false;
	}

	IEnumerator FadeOutImage(SpriteRenderer renderer)
	{
		Color color = renderer.color;
		Color startColor = color;

		//Fade out using gradient over time
		float elapsed = 0;
		while(elapsed < lifeTime)
		{
			color = fadeOutGradient.Evaluate(elapsed / lifeTime);

			renderer.color = color;

			yield return null;
			elapsed += Time.deltaTime;
		}

		renderer.color = startColor;

		renderer.gameObject.SetActive(false);
	}
}
