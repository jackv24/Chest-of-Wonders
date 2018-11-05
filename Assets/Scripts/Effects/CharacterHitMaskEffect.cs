using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitMaskEffect : MonoBehaviour
{
	public SpriteRenderer characterSprite;

	public SpriteMask spriteMask;
	public SpriteRenderer maskSprite;

	private CharacterStats owner;

	public void SetOwner(CharacterStats owner)
	{
		this.owner = owner;

		if(characterSprite)
		{
			characterSprite.sharedMaterial = owner.Graphic.sharedMaterial;
		}
	}

	private void Update()
	{
		if(owner && characterSprite)
		{
			characterSprite.sprite = owner.Graphic.sprite;
			characterSprite.transform.position = owner.transform.position;
			characterSprite.transform.rotation = owner.transform.rotation;
		}

		if(spriteMask && maskSprite)
		{
			spriteMask.sprite = maskSprite.sprite;
		}
	}
}
