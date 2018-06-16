using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMarker : MonoBehaviour
{
	public virtual Vector2 SpawnPosition => (Vector2)transform.position + spawnOffset;

	[SerializeField]
	private Vector2 spawnOffset;
}
