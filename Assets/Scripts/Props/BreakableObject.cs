using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
	public SpriteRenderer[] disableSprites;
	public SpriteRenderer[] enableSprites;

	[Header("Effects")]
	public GameObject[] breakEffectPrefabs;
	public Vector2 effectSpawnOffset;

	[Space()]
	public CameraShakeTarget hitShake;

	[Header("Sounds")]
	public SoundEventType breakSound;

	[Header("Saving/Loading")]
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
			Break(damageProperties);

			return true;
		}
		else
		{
			return false;
		}
	}

	private void Break(DamageProperties damageProperties)
	{
		foreach (GameObject effectPrefab in breakEffectPrefabs)
		{
			GameObject obj = ObjectPooler.GetPooledObject(effectPrefab);
			obj.transform.position = transform.TransformPoint(effectSpawnOffset);

			obj.GetComponent<SetRotationFromBreakableObject>()?.SetRotation(damageProperties.direction);
		}

		SetSpriteRenderers(true);

		breakSound.Play(transform.position);
		hitShake.DoShake();

		isBroken = true;

		if (usePersistentObject)
			persistentObject.SaveState(true);
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
