﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    //Where to load the dialog json file
    public TextAsset dialogueFile;
    public Color windowColor = Color.grey;

    [Space()]
    //How far offset from the gameobject should it be (in world space)
    public Vector2 boxOffset;

    private PlayerActions playerActions;
    private bool inRange = false;
    [HideInInspector]
    public bool rangeToggle = false;

    public float range = 2f;
    public float talkRange = 1.0f;
    private GameObject player;

    [Space()]
    public GameObject graphic;
    public bool facePlayer = true;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        playerActions = ControlManager.GetPlayerActions();
    }

    private void Update()
    {
        if (player)
        {
            if (Vector2.Distance(player.transform.position, transform.position) <= range)
                inRange = true;
            else
                inRange = false;
        }


        if (inRange)
        {
            if (rangeToggle)
                DialogueBox.Instance.ShowPromptIcon((Vector2)transform.position + boxOffset);

            rangeToggle = false;

            //If interact buttons is pressed in range...and the player "can move" (can perform actions outside of UI)
            if (playerActions.Interact.WasPressed && dialogueFile != null && GameManager.instance.CanDoActions)
            {
                //Stop them moving and open dialogue
                DialogueBox.Instance.OpenDialogue(dialogueFile, gameObject.name);

                //If desired, face the player while speaking
                if (facePlayer && graphic)
                {
                    //Make x scale -1 or 1 to flip the sprite to face the player
                    Vector3 scale = graphic.transform.localScale;
                    scale.x = Mathf.Sign(player.transform.position.x - transform.position.x);
                    graphic.transform.localScale = scale;
                }

                if (player)
                    StartCoroutine("MovePlayer");
            }
        }
        else
        {
            if (!rangeToggle)
                DialogueBox.Instance.HidePromptIcon();

            rangeToggle = true;
        }
    }

    IEnumerator MovePlayer()
    {
        float sign = Mathf.Sign(player.transform.position.x - transform.position.x);
        float targetPos = transform.position.x + sign * talkRange;

		//Don't bother moving when the difference is not noticeable
        if (Mathf.Abs(player.transform.position.x - targetPos) < 0.5f)
            yield return null;

        //Get character move and cache move speed
        CharacterMove characterMove = player.GetComponent<CharacterMove>();
        float moveSpeed = characterMove.moveSpeed;

        //Allow movement at half speed
        characterMove.ignoreCanMove = true;
        characterMove.moveSpeed = moveSpeed * 0.5f;

        //Stop camera jerkiness
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        float camDist = 0;

        if(cam)
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
    }

    public void MoveSpeaker(float x)
    {
        StartCoroutine("Move", x);
    }

    IEnumerator Move(float x)
    {
        float sign = Mathf.Sign(x);
        float targetPos = transform.position.x + x;

        //Get character move and cache move speed
        CharacterMove characterMove = GetComponent<CharacterMove>();

        //While player is not at target position (according to sign)
        while ((sign < 0 && transform.position.x > targetPos) || (sign > 0 && transform.position.x < targetPos))
        {
            //Move player
            characterMove.Move(sign);
            yield return new WaitForEndOfFrame();
        }

        //Face back towards speaker
        characterMove.Move(-sign);

        yield return new WaitForEndOfFrame();

        //Stop moving
        characterMove.Move(0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.DrawWireSphere(transform.position + (Vector3)boxOffset, 0.5f);

        Gizmos.DrawLine(new Vector3(-talkRange, 1.0f) + transform.position, new Vector3(-talkRange, 0) + transform.position);
        Gizmos.DrawLine(new Vector3(talkRange, 1.0f) + transform.position, new Vector3(talkRange, 0) + transform.position);
    }
}
