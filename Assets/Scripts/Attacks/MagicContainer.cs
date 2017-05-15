using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicContainer : MonoBehaviour
{
    public MagicAttack attack;

    [Space()]
    public float pickupRange = 2.0f;
    public float buttonHoldTime = 1.0f;
    private float buttonHeldTime = 0;

    [Header("Fade Out")]
    public SpriteRenderer[] fadeGraphics;
    [Space()]
    [Range(0, 1f)]
    public float outOfRangeOpacity = 0.5f;
    [Range(0, 1f)]
    public float inRangeOpacity = 1.0f;
    public float fadeTime = 0.5f;
    private bool inRange = false;

    [Header("Float Anim")]
    public Transform[] toMove;
    private float animTime = 0f;
    [Space()]
    public AnimationCurve moveX;
    public float magnitudeX = 0.5f;
    public float animLengthX = 2.0f;
    [Space()]
    public AnimationCurve moveY;
    public float magnitudeY = 0.5f;
    public float animLengthY = 2.0f;
    [Space()]
    public float buttonHeldSpeedMultiplier = 2.0f;
    private float currentSpeedMultiplier = 1.0f;

    private PlayerActions playerActions;

    void Start()
    {
        playerActions = ControlManager.GetPlayerActions();
    }

    private void OnEnable()
    {
        buttonHeldTime = 0;
        currentSpeedMultiplier = 1.0f;
        animTime = 0;
    }

    void Update()
    {
        //Get all colliders in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        inRange = false;

        foreach(Collider2D col in cols)
        {
            //If player is in range
            if (col.tag == "Player")
            {
                inRange = true;

                if (playerActions.AbsorbMagic.IsPressed)
                {
                    //If button is held for long enough, absorb attack
                    if (buttonHeldTime >= buttonHoldTime)
                        Absorb(col.gameObject);
                    else
                    {
                        //Count time button is held
                        buttonHeldTime += Time.deltaTime;

                        currentSpeedMultiplier = Mathf.Lerp(1.0f, buttonHeldSpeedMultiplier, buttonHeldTime / buttonHoldTime);
                    }
                }
                else
                {
                    buttonHeldTime = 0;

                    currentSpeedMultiplier = 1.0f;
                }
            }
        }

        foreach(SpriteRenderer r in fadeGraphics)
        {
            r.color = Color.Lerp(r.color,
                new Color(
                    r.color.r,
                    r.color.g,
                    r.color.b,
                    inRange ? inRangeOpacity : outOfRangeOpacity),
                (1/fadeTime) * Time.deltaTime);
        }

        animTime += Time.deltaTime;

        foreach(Transform t in toMove)
        {
            t.position = new Vector3(
                moveX.Evaluate((animTime * currentSpeedMultiplier) / animLengthX) * magnitudeX,
                moveY.Evaluate((animTime * currentSpeedMultiplier) / animLengthY) * magnitudeY,
                0) + transform.position;
        }
    }

    void Absorb(GameObject player)
    {
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        if(playerAttack)
        {
            //If slot 2 is empty move current attack into that slot to fit the new one
            if (playerAttack.magicSlot2.attack == null)
                playerAttack.SwitchMagic();

            //Overwrite attack in slot 1
            playerAttack.magicSlot1.attack = attack;
            playerAttack.magicSlot1.currentMana = attack.manaAmount;

            //Make sure UI is updated
            playerAttack.UpdateMagic();

            //Destroy container
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
