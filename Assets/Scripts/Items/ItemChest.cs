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

	private Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		persistentObject.OnStateLoaded += (bool activated) =>
		{
			opened = activated;

			if (opened)
			{
				//Skip to the end of play animation
				animator?.Play("Open", 0, 1.0f);
			}
		};

		persistentObject.Setup(gameObject);
	}

	public void Interact()
	{
		opened = true;
		InteractManager.RemoveInteractible(this);

		if(PlayerInventory.Instance && containingItem)
		{
			PlayerInventory.Instance.AddItem(containingItem);

			persistentObject.SaveState(opened);

			animator?.Play("Open");
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

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position + Vector3.up * promptHeight, "Interact Icon");
	}
}
