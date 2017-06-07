using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicContainer : MonoBehaviour
{
    public MagicAttack attack;

    private bool isAbsorbing = false;
    private float buttonHoldTime;
    private float buttonHeldTime;
    private PlayerAttack playerAttack;

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
    private Vector2[] initialPos;
    [Space()]
    public float buttonHeldSpeedMultiplier = 2.0f;
    private float currentSpeedMultiplier = 1.0f;

    private void OnEnable()
    {
        isAbsorbing = false;

        currentSpeedMultiplier = 1.0f;
        animTime = 0;
    }

    void Start()
    {
        initialPos = new Vector2[toMove.Length];

        for(int i = 0; i < initialPos.Length; i++)
        {
            initialPos[i] = toMove[i].localPosition;
        }
    }

    void Update()
    {
        if (isAbsorbing)
        {
            //If button is held for long enough, absorb attack
            if (buttonHeldTime >= buttonHoldTime)
                Absorb();
            else
            {
                //Count time button is held
                buttonHeldTime += Time.deltaTime;

                currentSpeedMultiplier = Mathf.Lerp(1.0f, buttonHeldSpeedMultiplier, buttonHeldTime / buttonHoldTime);
            }
        }

        //Fade colours for in/out of range
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

        //Animate position of graphic transforms using curves
        animTime += Time.deltaTime;

        for(int i = 0; i < toMove.Length; i++)
        {
            toMove[i].localPosition = new Vector3(
                moveX.Evaluate((animTime * currentSpeedMultiplier) / animLengthX) * magnitudeX,
                moveY.Evaluate((animTime * currentSpeedMultiplier) / animLengthY) * magnitudeY,
                0) + (Vector3)initialPos[i];
        }
    }

    public void Highlight(bool value)
    {
        inRange = value;
    }

    public void StartAbsorb(PlayerAttack playerAttack)
    {
        isAbsorbing = true;

        this.playerAttack = playerAttack;

        buttonHoldTime = playerAttack.buttonHoldTime;
        buttonHeldTime = 0;
    }

    public void CancelAbsorb()
    {
        isAbsorbing = false;

        buttonHeldTime = 0;
        currentSpeedMultiplier = 1.0f;
    }

    void Absorb()
    {
        if (playerAttack)
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
}
