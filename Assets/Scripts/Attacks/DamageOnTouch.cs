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

    [Header("Effects")]
    [Tooltip("The effect to show when something is hit.")]
    public GameObject hitEffect;

    [Space()]
    public float screenShakeAmount = 0.5f;
    public float knockBackAmount = 10f;
    public Vector2 knockBackCentreOffset = -Vector2.up;

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

        Vector2 pos = (Vector2)transform.position + damageBoxPos;

        Collider2D[] cols = Physics2D.OverlapBoxAll(pos, damageBoxSize, 0, damageLayers);

        foreach (Collider2D other in cols)
        {
            //Make sure this gameobject was not damaged recently
            if (!onCoolDown.Contains(other.gameObject))
            {
                //Get character references
                IDamageable damageable = other.GetComponent<IDamageable>();
                CharacterMove move = other.GetComponent<CharacterMove>();

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
                        //Calculate centre point between colliders to show hit effect
                        Vector3 centre = (pos + (Vector2)other.bounds.center) / 2;

                        //Show hit effect at centre of colliders (with object pooling)
                        GameObject effect = ObjectPooler.GetPooledObject(hitEffect);
                        effect.transform.position = centre;

                        //Knockback if amount is more than 0
                        if (move && knockBackAmount > 0)
                            move.Knockback((Vector2)transform.position + knockBackCentreOffset, knockBackAmount);

                        //Offset randomly (screen shake effect)
                        Vector2 camOffset = new Vector2(Random.Range(-1f, 1f) * screenShakeAmount, Random.Range(-1f, 1f) * screenShakeAmount);
                        Camera.main.transform.position += (Vector3)camOffset;

                        onCoolDown.Add(other.gameObject);
                        StartCoroutine("RemoveFromCooldown", other.gameObject);
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
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + (Vector3)damageBoxPos, damageBoxSize);
    }
}
