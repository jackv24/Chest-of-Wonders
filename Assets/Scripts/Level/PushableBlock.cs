using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBlock : MonoBehaviour
{
	[Tooltip("MUST BE UNIQUE")]
	public int uniqueID = 0;
	[Tooltip("Position will persist between level loads")]
	public bool keepPosition = true;

	private Transform player;

	[Tooltip("How fast the block is pushed.")]
	public float moveSpeed = 2.0f;
	public float pushDistance = 1.0f;

	[Space()]
	public float gravity = -9.8f;

	[Space()]
	public float groundedRayDist = 2.0f;

	private bool pushing = false;

	private PlayerActions playerActions;

	private CharacterMove characterMove;
	private CharacterAnimator characterAnimator;

	private Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		playerActions = ControlManager.GetPlayerActions();

		//Make sure block starts on the grid
		ReturnToGrid();

		if(keepPosition)
			transform.position = SaveManager.instance.GetObjectPosition(uniqueID, transform.position);
	}

	private void Update()
	{
		//Make sure the player is on the ground and not currently pushing the block
		if(!pushing && player && characterMove && characterMove.isGrounded && player.position.y < transform.position.y)
		{
			float direction = 0;

			Vector3 offset = transform.position - player.position;

			//See if player is pushing into the block
			if (playerActions.Left.IsPressed && offset.x < 0)
				direction = -1;
			else if (playerActions.Right.IsPressed && offset.x > 0)
				direction = 1;

			//If pushing into the block, move it in that direction
			if (direction != 0)
			{
				pushing = true;
				StartCoroutine(MoveBlock(direction));
			}
		}
	}

	IEnumerator MoveBlock(float direction)
	{
		//Prevent input
		GameManager.instance.gameRunning = false;
		characterMove.ignoreCanMove = true;

		//Cache and set move speed
		float m = characterMove.moveSpeed;
		characterMove.moveSpeed = moveSpeed;

		//Start pushing animation
		if (characterAnimator)
			characterAnimator.animator.SetBool("pushBlock", true);

		bool running = true;

		while (running)
		{
			running = false;

			//t = d*(1/v)
			float pushTime = pushDistance*(1 / moveSpeed);
			float elapsedTime = 0;

			body.isKinematic = false;

			//Push block until target X is reached
			while (elapsedTime < pushTime)
			{
				characterMove.Move(direction);

				transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;
			}

			body.isKinematic = true;
			body.velocity = Vector2.zero;

			characterMove.Move(0);

			//Make sure block stays on grid
			ReturnToGrid();

			//Continue pushing if buttons are held
			if ((direction < 0 && playerActions.Left.IsPressed) || (direction > 0 && playerActions.Right.IsPressed))
				running = true;

			float fallSpeed = 0;

			bool checkFall = true;
			while (checkFall)
			{
				RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down);

				if (hits.Length < 1)
					break;

				foreach (RaycastHit2D hit in hits)
				{
					//If ray within range, block does not need to fall
					if (hit.collider.gameObject != gameObject && hit.distance <= groundedRayDist)
						checkFall = false;
				}

				//If block has fallen
				if (checkFall)
				{
					//Accelerate and move down
					fallSpeed -= gravity * Time.deltaTime;
					transform.position += Vector3.down * fallSpeed * Time.deltaTime;

					//Stop pushing after falling
					player = null;
					running = false;

					if (characterAnimator)
						characterAnimator.animator.SetBool("pushBlock", false);
				}

				yield return new WaitForEndOfFrame();
			}
		}

		//Stop animation
		if (characterAnimator)
			characterAnimator.animator.SetBool("pushBlock", false);

		ReturnToGrid();

		//Resume input
		GameManager.instance.gameRunning = true;
		characterMove.ignoreCanMove = false;

		//Restore move speed
		characterMove.moveSpeed = m;

		pushing = false;

		if(keepPosition)
			SaveManager.instance.SetObjectPosition(uniqueID, transform.position);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.collider.tag == "Player")
		{
			player = collision.transform;

			characterAnimator = player.GetComponent<CharacterAnimator>();
			characterMove = player.GetComponent<CharacterMove>();
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.collider.tag == "Player" && player)
		{
			player = null;
		}
	}

	void ReturnToGrid()
	{
		Vector3 pos = transform.position;
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		transform.position = pos;
	}
}
