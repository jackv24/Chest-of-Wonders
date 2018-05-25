using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeRegion : MonoBehaviour
{
	public GroundType groundType;

	private void Reset()
	{
		//Should use a collider, but it doesn't have to be a box collider
		Collider2D col = GetComponent<Collider2D>();
		if(!col)
		{
			col = gameObject.AddComponent<BoxCollider2D>();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		CharacterGroundEffects effects = collision.GetComponent<CharacterGroundEffects>();
		if(effects)
		{
			effects.AddGroundTypeRegion(this);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		CharacterGroundEffects effects = collision.GetComponent<CharacterGroundEffects>();
		if (effects)
		{
			effects.RemoveGroundTypeRegion(this);
		}
	}
}
