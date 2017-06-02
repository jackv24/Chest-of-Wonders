using System.Collections;
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
            //if (rangeToggle)
            //    DialogueBox.instance.ShowIcon(true, this);

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
            //if(!rangeToggle)
            //    DialogueBox.instance.ShowIcon(false);

            rangeToggle = true;
        }
    }

    IEnumerator MovePlayer()
    {
        float sign = Mathf.Sign(player.transform.position.x - transform.position.x);
        float targetPos = transform.position.x + sign * talkRange;

        //Get character move and cache move speed
        CharacterMove characterMove = player.GetComponent<CharacterMove>();
        float moveSpeed = characterMove.moveSpeed;

        //Allow movement at half speed
        characterMove.ignoreCanMove = true;
        characterMove.moveSpeed = moveSpeed * 0.5f;

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

        //Stop moving
        characterMove.Move(0);

        //Restore cached values
        characterMove.ignoreCanMove = false;
        characterMove.moveSpeed = moveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.DrawWireSphere(transform.position + (Vector3)boxOffset, 0.5f);

        Gizmos.DrawLine(new Vector3(-talkRange, 1.0f) + transform.position, new Vector3(-talkRange, 0) + transform.position);
        Gizmos.DrawLine(new Vector3(talkRange, 1.0f) + transform.position, new Vector3(talkRange, 0) + transform.position);
    }
}
