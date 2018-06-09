using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
	public SpriteRenderer[] disableSprites;
	public SpriteRenderer[] enableSprites;

	public GameObject[] breakEffectPrefabs;

	public Vector2 effectSpawnOffset;

	[Space()]
	public bool usePersistentObject = false;
	public PersistentObject persistentObject;

	private bool isBroken = false;

	void Start()
	{
		SetSpriteRenderers(false);

		if (usePersistentObject)
		{
			persistentObject.OnStateLoaded += (activated) =>
			{
				if (activated)
				{
					isBroken = true;
					SetSpriteRenderers(true);
				}
			};

			persistentObject.Setup(gameObject);
		}
	}

	public bool TakeDamage(DamageProperties damageProperties)
	{
		//Can't take damage if already broken
		if (isBroken)
			return false;

		//Only take damage from regular type attacks (not projectiles)
		if(damageProperties.type == DamageType.Regular)
		{
			foreach(GameObject effectPrefab in breakEffectPrefabs)
			{
				GameObject obj = ObjectPooler.GetPooledObject(effectPrefab);
				obj.transform.position = transform.TransformPoint(effectSpawnOffset);
			}

			if(usePersistentObject)
				persistentObject.SaveState(true);

			SetSpriteRenderers(true);
			isBroken = true;

			return true;
		}
		else
		{
			return false;
		}
	}

	private void SetSpriteRenderers(bool isBroken)
	{
		foreach (SpriteRenderer sprite in enableSprites)
			sprite.enabled = isBroken;

		foreach (SpriteRenderer sprite in disableSprites)
			sprite.enabled = !isBroken;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(effectSpawnOffset, 0.25f);
	}
}
