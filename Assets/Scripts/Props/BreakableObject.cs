using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
	public GameObject[] breakEffectPrefabs;

	public Vector2 effectSpawnOffset;

	[Space()]
	public bool usePersistentObject = false;
	public PersistentObject persistentObject;

	void Start()
	{
		if (usePersistentObject)
		{
			persistentObject.OnStateLoaded += (activated) =>
			{
				if (activated)
					gameObject.SetActive(false);
			};

			persistentObject.Setup(gameObject);
		}
	}

	public bool TakeDamage(DamageProperties damageProperties)
	{
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

			gameObject.SetActive(false);

			return true;
		}
		else
		{
			return false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(effectSpawnOffset, 0.25f);
	}
}
