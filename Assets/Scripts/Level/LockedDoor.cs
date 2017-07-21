using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
	[Tooltip("MUST BE UNIQUE (unless another door should be unlocked as well)")]
	public int uniqueID = 0;

	[Tooltip("Item needed to open door.")]
	public InventoryItem requiredItem;
	public bool consumeItem = true;
	[Space()]
	public GameObject enableObject;
	[Space()]
	public GameObject lockedPadlock;
	public GameObject openPadlock;
	public float openDoorTime = 0.5f;

	[Space()]
	public Vector2 centreOffset;
	public float openRange = 1.0f;
	public float stepBackDistance = 1.5f;
	public float moveSpeedMultiplier = 0.5f;

	private PlayerActions playerActions;
	private Transform player;
	private PlayerInventory inventory;

	void Start()
	{
		playerActions = ControlManager.GetPlayerActions();

		player = GameManager.instance.player.transform;

		if (player)
			inventory = player.GetComponent<PlayerInventory>();

		if (enableObject)
			enableObject.SetActive(false);

		//Check if this item has already been picked up
		if (SaveManager.instance.IsDoorOpened(uniqueID))
		{
			if (enableObject)
				enableObject.SetActive(true);

			if (lockedPadlock)
				lockedPadlock.SetActive(false);

			gameObject.SetActive(false);
		}

		if (openPadlock)
			openPadlock.SetActive(false);
	}

	void Update()
	{
		if (inventory)
		{
			if (Vector2.Distance(transform.position + (Vector3)centreOffset, player.position) <= openRange)
			{
				//If interact was pressed and if item is in inventory (consume if it is)
				if ((playerActions.Interact.WasPressed || playerActions.Up.WasPressed) && inventory.CheckItem(requiredItem, true))
					OpenDoor();
			}
		}
	}

	void OpenDoor()
	{
		//Save that this door has been opened
		SaveManager.instance.SetOpenedDoor(uniqueID);

		StartCoroutine("MovePlayerOpenDoor");
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position + (Vector3)centreOffset, openRange);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position + (Vector3)centreOffset, stepBackDistance);
	}

	IEnumerator MovePlayerOpenDoor()
	{
		if (stepBackDistance > 0)
		{
			float sign = Mathf.Sign(player.transform.position.x - transform.position.x);
			float targetPos = transform.position.x + sign * stepBackDistance;

			//Don't bother moving when the difference is not noticeable
			if (Mathf.Abs(player.transform.position.x - targetPos) < 0.5f)
				yield return null;

			//Enemy and player input is paused while door is opening
			GameManager.instance.gameRunning = false;

			//Get character move and cache move speed
			CharacterMove characterMove = player.GetComponent<CharacterMove>();
			float moveSpeed = characterMove.moveSpeed;

			//Allow movement at half speed
			characterMove.ignoreCanMove = true;
			characterMove.moveSpeed = moveSpeed * moveSpeedMultiplier;

			//Stop camera jerkiness
			CameraFollow cam = FindObjectOfType<CameraFollow>();
			float camDist = 0;

			if (cam)
			{
				camDist = cam.lookAhead;
				cam.lookAhead = 0;
			}

			//While player is not at target position (according to sign)
			while ((sign < 0 && player.transform.position.x > targetPos) || (sign > 0 && player.transform.position.x < targetPos))
			{
				//Move player
				characterMove.Move(sign);
				yield return new WaitForEndOfFrame();
			}

			//Face back towards speaker
			characterMove.Move(-sign);

			yield return new WaitForEndOfFrame();

			//Restore camera distance
			if (cam)
				cam.lookAhead = camDist;

			//Stop moving
			characterMove.Move(0);

			//Restore cached values
			characterMove.ignoreCanMove = false;
			characterMove.moveSpeed = moveSpeed;

			GameManager.instance.gameRunning = true;
		}

		if (enableObject)
		{
			enableObject.SetActive(true);
			enableObject.SendMessage("SetInDoor");
		}

		if (lockedPadlock)
			lockedPadlock.SetActive(false);

		if(openPadlock)
		{
			openPadlock.SetActive(true);
			yield return new WaitForSeconds(openDoorTime);
			openPadlock.SetActive(false);
		}

		//Open door
		gameObject.SetActive(false);
	}
}
