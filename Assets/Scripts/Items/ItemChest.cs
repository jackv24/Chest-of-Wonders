using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour, IInteractible
{
	public InteractType InteractType { get { return InteractType.Open; } }

	public float promptHeight = 1.0f;

	public InventoryItem containingItem;

	public PersistentObject persistentObject;

	private bool opened = false;
	private PlayerInventory inventory;

	private Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		inventory = FindObjectOfType<PlayerInventory>();

		persistentObject.GetID(gameObject);
		persistentObject.LoadState(ref opened);

		if(opened)
		{
			Open();
		}
	}

	public void Interact()
	{
		opened = true;
		InteractManager.RemoveInteractible(this);

		if(inventory && containingItem)
		{
			inventory.AddItem(containingItem);

			persistentObject.SaveState(opened);

			Open();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(!opened)
			InteractManager.AddInteractible(this, transform.position, Vector3.up * promptHeight);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(!opened)
			InteractManager.RemoveInteractible(this);
	}

	void Open()
	{
		if (animator)
			animator.SetBool("open", true);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position + Vector3.up * promptHeight, "Interact Icon");
	}
}
