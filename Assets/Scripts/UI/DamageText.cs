using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    //Static instance as this should only be attached to one canvas
    public static DamageText instance;

    //Prefab to spawn for damage text
    public GameObject textPrefab;

    [Space()]
    public Sprite damageUp;
    public Sprite damageDown;

    //Animation
    [Space()]
    public float textLifeTime = 2f;
    public AnimationCurve verticalMovement = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Gradient colourOverLifeTime;

    private void Awake()
    {
        instance = this;
    }

    public void ShowDamageText(Vector2 characterPos, int amount, int effectiveNess)
    {
        //Get damage text and make sure it is parented to this canvas
        GameObject obj = ObjectPooler.GetPooledObject(textPrefab);
        obj.transform.SetParent(transform, false);

        //Set the world position, so that it stays there on the canvas
        KeepWorldPosOnCanvas posKeeper = obj.GetComponent<KeepWorldPosOnCanvas>();
        posKeeper.worldPos = characterPos;

        //Set damage text
        Text damageText = obj.GetComponent<Text>();
        damageText.text = amount.ToString();

        if(effectiveNess != 0)
        {
            Image img = damageText.GetComponentInChildren<Image>();

            if(img)
            {
                img.sprite = effectiveNess > 0 ? damageUp : damageDown;
                img.color = Color.white;
            }
        }

        StartCoroutine("AnimateText", obj);
    }

    IEnumerator AnimateText(GameObject textObject)
    {
        float lifeTime = 0;

        KeepWorldPosOnCanvas posKeeper = textObject.GetComponent<KeepWorldPosOnCanvas>();
        Vector2 initialPos = posKeeper.worldPos;
        Text text = textObject.GetComponent<Text>();

        while (lifeTime < textLifeTime)
        {
            yield return new WaitForEndOfFrame();
            lifeTime += Time.deltaTime;

            float percent = lifeTime / textLifeTime;

            posKeeper.worldPos = initialPos + new Vector2(0, verticalMovement.Evaluate(percent));
            text.color = colourOverLifeTime.Evaluate(percent);
        }

        textObject.SetActive(false);
    }
}
