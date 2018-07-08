using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
	public int InsideCount { get { return insideTrigger.Count; } }

	private List<GameObject> insideTrigger = new List<GameObject>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!insideTrigger.Contains(collision.gameObject))
			insideTrigger.Add(collision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (insideTrigger.Contains(collision.gameObject))
			insideTrigger.Remove(collision.gameObject);
	}
}
