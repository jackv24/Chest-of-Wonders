using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
	public int InsideCount
    {
        get
        {
            if (!requireLineOfSight)
                return insideTrigger.Count;

            // Use pre-calculated line of sight values
            int visibleCount = 0;
            for (int i = 0; i < isVisibles.Count; i++)
            {
                if (isVisibles[i])
                    visibleCount++;
            }
            return visibleCount;
        }
    }

    [SerializeField]
    private bool requireLineOfSight;

    [SerializeField]
    private LayerMask raycastLayerMask;

    [SerializeField]
    private Vector2 raycastOrigin;

    private List<GameObject> insideTrigger = new List<GameObject>();
    private List<bool> isVisibles = new List<bool>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(raycastOrigin, 0.1f);
    }

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

    private void FixedUpdate()
    {
        if (requireLineOfSight)
            UpdateVisibles();
    }

    private void UpdateVisibles()
    {
        // Check line of sight for all objects inside (do once and use results later)
        isVisibles.Clear();
        foreach (var obj in insideTrigger)
        {
            Vector3 start = transform.TransformPoint(raycastOrigin);
            Vector3 end = obj.transform.position;

            RaycastHit2D hit = Physics2D.Linecast(start, end, raycastLayerMask);
            isVisibles.Add(hit.collider == null);

            Debug.DrawLine(start, end);
        }
    }
}
