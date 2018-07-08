using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    [Header("Attacks")]
    public StringGameObjectDictionary attackColliders;
	private string currentAttackCollider;

    [System.Serializable]
    public class ProjectileAttack
    {
        public Transform spawnPoint;
        public Projectile projectilePrefab;
    }

    [Header("Projectile Attacks")]
    public ProjectileAttack[] projectileAttacks;

    private CharacterMove characterMove;
	private CharacterGroundEffects characterGroundEffects;

    void Awake()
    {
        characterMove = GetComponentInParent<CharacterMove>();
		characterGroundEffects = GetComponentInParent<CharacterGroundEffects>();
    }

	public void SetCurrentAttackCollider(string id)
	{
		currentAttackCollider = id;
	}

	public void EnableCurrentAttackCollider()
	{
		SetCurrentAttackColliderActive(true);
	}

	public void DisableCurrentAttackCollider()
	{
		SetCurrentAttackColliderActive(false);
	}

	private void SetCurrentAttackColliderActive(bool value)
	{
		if (!string.IsNullOrEmpty(currentAttackCollider) && attackColliders.ContainsKey(currentAttackCollider))
		{
			attackColliders[currentAttackCollider].SetActive(value);
		}
	}

	public void ShakeScreen(float amount)
	{
		//Offset randomly (screen shake effect)
		Vector2 camOffset = new Vector2(Random.Range(-1f, 1f) * amount, Random.Range(-1f, 1f) * amount);
		Camera.main.transform.position += (Vector3)camOffset;
	}

	public void GroundImpact(float amount)
	{
		//Offset randomly (screen shake effect)
		Vector2 camOffset = Vector2.down * amount;
		Camera.main.transform.position += (Vector3)camOffset;
	}

    public void FireProjectileHorizontal(int index)
    {
        if(index < projectileAttacks.Length)
        {
            ProjectileAttack attack = projectileAttacks[index];

            if(attack.spawnPoint && attack.projectilePrefab)
            {
                GameObject obj = ObjectPooler.GetPooledObject(attack.projectilePrefab.gameObject);
                obj.transform.position = attack.spawnPoint.position;

                Projectile proj = obj.GetComponent<Projectile>();
                proj.SetOwner(characterMove.gameObject);
                proj.Fire(attack.spawnPoint.forward);
            }
        }
    }

	public void PlayFootstep()
	{
		if (characterGroundEffects)
			characterGroundEffects.PlayFootstep();
	}
}
