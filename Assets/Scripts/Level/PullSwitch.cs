using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullSwitch : MonoBehaviour
{
	public float pullDistance = 1.0f;

	public float pullTime = 2.0f;
	public AnimationCurve pullCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	public Transform playerAttachPoint;

	[Space()]
	public Transform sparkSpawnPoint;
	public GameObject sparkEffect;
	public float sparkDelay = 0.0f;

	[Space()]
	public GameObject spawnObject;
	public Transform spawnPoint;
	private GameObject spawnedObject;

	private bool pulled = false;
	private bool shouldSave = true;

	public PersistentObject persistentObject;

	void Start()
	{
		persistentObject.GetID(gameObject);
		persistentObject.LoadState(ref pulled);

		if (pulled)
			transform.position += Vector3.down * pullDistance;

		//Switch pull state should not be saved if level is unloaded because of death
		GameManager.instance.OnGameOver += delegate { shouldSave = false; };
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player" && !pulled)
		{
			pulled = true;

			StartCoroutine(PullOut(collision.gameObject));
		}
	}

	private void OnDisable()
	{
		//Make sure spawned object has been picked up
		if (pulled && shouldSave)
		{
			if (!spawnedObject || !spawnedObject.activeSelf)
			{
				persistentObject.SaveState(pulled);
			}
		}
	}

	IEnumerator PullOut(GameObject player)
	{
		CharacterMove characterMove = player.GetComponent<CharacterMove>();
		Animator animator = player.GetComponentInChildren<Animator>();

		float direction = Mathf.Sign(animator.transform.localScale.x);

		//Stop enemies moving
		GameManager.instance.gameRunning = false;

		//Stop player from moving
		characterMove.scriptControl = false;
		characterMove.velocity = Vector2.zero;

		Vector3 initialPos = transform.localPosition;
		Vector3 targetPos = initialPos + Vector3.down * pullDistance;

		animator.Play("Pull Switch");

		//Offset on x depending on the direction the player is facing
		Vector3 offset = playerAttachPoint.localPosition;
		offset.x *= direction;

		float elapsedTime = 0;
		bool spawnedSpark = false;

		while (elapsedTime <= pullTime)
		{
			//Lerp position using animation curve
			transform.localPosition = Vector3.Lerp(initialPos, targetPos, pullCurve.Evaluate(elapsedTime / pullTime));

			//Keep player at attach point
			player.transform.position = transform.position + offset;

			if(!spawnedSpark && elapsedTime >= sparkDelay)
			{
				spawnedSpark = true;

				if (sparkEffect)
				{
					GameObject obj = ObjectPooler.GetPooledObject(sparkEffect);
					obj.transform.position = sparkSpawnPoint.position;
				}
			}

			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}

		animator.Play("Locomotion");

		if(spawnPoint && spawnObject)
		{
			GameObject obj = Instantiate(spawnObject, spawnPoint);
			obj.transform.localPosition = Vector3.zero;

			obj.transform.parent = null;

			spawnedObject = obj;
		}

		//Restore game state
		GameManager.instance.gameRunning = true;
		characterMove.scriptControl = true;
	}
}
