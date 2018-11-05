using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doorway : SpawnMarker
{
	public override Vector2 SpawnPosition => (Vector2)transform.position + new Vector2(0, exitOffset);

	public float exitOffset = 1.5f;

	[Space()]
	public string targetScene = "";
	public string targetDoor = "";

    [Space()]
    public float startDelay = 0.5f;
    private float startTime;

    [Space()]
    public bool useButton = false;
    private bool inDoor = false;

    private PlayerActions playerActions;

    void Start()
    {
        playerActions = ControlManager.GetPlayerActions();

        startTime = Time.time + startDelay;
    }

    void Update()
    {
        if(useButton && inDoor)
        {
            //If up button was pressed, use the door
            if(playerActions.Up.WasPressed && Time.time > startTime && GameManager.instance.CanDoActions)
                Use();
        }
    }

	public void SetInDoor()
	{
		inDoor = true;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //If a player enters this doorway, either use the doorway, or wait for button input in update
        if (other.tag == "Player" && Time.time > startTime)
        {
            inDoor = true;

			CharacterStats stats = other.GetComponent<CharacterStats>();

			//Can't go through door if dead (prevents being knocked through door and canceling death)
			if (stats && stats.CurrentHealth <= 0)
				return;

            if (!useButton)
                Use();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            inDoor = false;
    }

    void Use()
    {
        //Load level with player at position
        GameManager.instance.LoadLevel(targetScene, targetDoor);
    }

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(exitOffset, 0), 0.25f);
    }

	private void OnDrawGizmos()
	{
		BoxCollider2D col = GetComponent<BoxCollider2D>();

		Gizmos.color = new Color(0, 1, 0, 0.5f);
		Gizmos.DrawCube((Vector2)transform.position + col.offset, col.size);

		GUIStyle textStyle = new GUIStyle();
		textStyle.normal.textColor = Color.white;
		textStyle.fontSize = Mathf.RoundToInt(24 / UnityEditor.HandleUtility.GetHandleSize(transform.position));

		UnityEditor.Handles.Label(
			(Vector2)transform.position + new Vector2(-col.size.x / 2, col.offset.y + col.size.y / 2 + 1.0f),
			$"<color=grey>Scene: </color>{targetScene}\n<color=grey>Door: </color>{targetDoor}",
			textStyle);
	}
#endif
}
