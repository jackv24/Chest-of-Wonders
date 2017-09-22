using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    [Header("Attacks")]
    public GameObject[] attackColliders;

	public void EnableAttackColliders(int index)
	{
		if (index < attackColliders.Length)
			attackColliders[index].SetActive(true);
	}

	public void DisableAttackColliders(int index)
	{
		if (index < attackColliders.Length)
			attackColliders[index].SetActive(false);
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
}
