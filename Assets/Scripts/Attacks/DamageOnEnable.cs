using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnable : MonoBehaviour
{
    public LayerMask damageLayer;

    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;
	public ElementManager.Element element;

	public bool useFacingDirection = true;

    [Header("Effects")]
    [Tooltip("The effect to show when something is hit.")]
    public GameObject hitEffect;

    [Space()]
    public float screenShakeAmount = 0.5f;
    public float knockBackAmount = 10f;
    public Vector2 knockBackCentreOffset = -Vector2.up;

    private List<GameObject> hitInSwing = new List<GameObject>();

	private Collider2D col;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	private void OnEnable()
    {
        hitInSwing.Clear();
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!hitInSwing.Contains(collider.gameObject))
		{
			//Keep track of what has already been hit (in case gameobject has multiple colliders)
			hitInSwing.Add(collider.gameObject);

			//Get character references
			IDamageable damageable = collider.GetComponentInParent<IDamageable>();
			CharacterMove move = collider.GetComponentInParent<CharacterMove>();

			//Remove health
			if (damageable != null)
			{
				//Only show damage effects if damage was actually taken
				if (damageable.TakeDamage(new DamageProperties
				{
					amount = amount,
					sourceElement = element,
					type = DamageType.Regular,
					direction = useFacingDirection ? (transform.lossyScale.x < 0 ? Vector2.left : Vector2.right) : Vector2.zero
				}))
				{
					//Calculate centre point between colliders to show hit effect
					Vector3 centre = (col.bounds.center + collider.bounds.center) / 2;

					//Show hit effect at centre of colliders (with object pooling)
					GameObject effect = ObjectPooler.GetPooledObject(hitEffect);
					effect.transform.position = centre;

					//Knockback if amount is more than 0
					if (move && knockBackAmount > 0)
						move.Knockback((Vector2)transform.position + knockBackCentreOffset, knockBackAmount);

					//Offset randomly (screen shake effect)
					Vector2 camOffset = new Vector2(Random.Range(-1f, 1f) * screenShakeAmount, Random.Range(-1f, 1f) * screenShakeAmount);
					Camera.main.transform.position += (Vector3)camOffset;
				}
			}
		}
	}
}
