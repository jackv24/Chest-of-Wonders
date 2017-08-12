﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doorway : MonoBehaviour
{
    public int doorwayID = 0;

    public float exitOffset = 1.5f;

    [Space()]
    public SceneField targetLevel;
    public int targetID = 0;

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
			if (stats && stats.currentHealth <= 0)
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
        GameManager.instance.LoadLevel(targetLevel, targetID);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(exitOffset, 0), 0.25f);
    }
}
