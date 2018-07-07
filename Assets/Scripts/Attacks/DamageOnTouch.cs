using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public LayerMask damageLayers;
    public Vector2 damageBoxPos;
    public Vector2 damageBoxSize = Vector2.one;

    [Space()]
    [Tooltip("How much damage to deal to everything this hits.")]
    public int amount = 10;
	public ElementManager.Element element;

    public float damageCooldown = 1f;

    private List<GameObject> onCoolDown = new List<GameObject>();

    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    private void OnEnable()
    {
        onCoolDown.Clear();
    }

    private void FixedUpdate()
    {
        //Character can not cause damage if it can not take damage
        if (characterStats && characterStats.damageImmunity)
            return;

        Vector2 pos = transform.TransformPoint(damageBoxPos);

        Collider2D[] cols = Physics2D.OverlapBoxAll(pos, transform.TransformVector(damageBoxSize), 0, damageLayers);

        foreach (Collider2D other in cols)
        {
            //Make sure this gameobject was not damaged recently
            if (!onCoolDown.Contains(other.gameObject))
            {
                //Get character references
                IDamageable damageable = other.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    //Attempt to remove health
                    if (damageable.TakeDamage(new DamageProperties
					{
						amount = amount,
						sourceElement = element,
						type = DamageType.Regular
					}))
                    {
						//TODO: Hit effects

                        onCoolDown.Add(other.gameObject);
                        StartCoroutine(RemoveFromCooldown(other.gameObject));
                    }
                }
            }
        }
    }

    IEnumerator RemoveFromCooldown(GameObject gameObject)
    {
        yield return new WaitForSeconds(damageCooldown);

        onCoolDown.Remove(gameObject);
    }

    void OnDrawGizmosSelected()
    {
		Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damageBoxPos, damageBoxSize);
    }

	public void SetActive(bool value)
	{
		enabled = value;
	}
}
