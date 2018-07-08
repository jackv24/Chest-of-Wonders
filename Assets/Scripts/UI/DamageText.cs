using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	public enum Effectiveness
	{
		Nuetral,
		Effective,
		Ineffective
	}

    //Static instance as this should only be attached to one canvas
    public static DamageText instance;

    //Prefab to spawn for damage text
    public GameObject textPrefab;

    [System.Serializable]
    public struct TextConfig
    {
        public Color textColor;
        public Color outlineColor;
        public int fontSize;
    }

    public TextConfig neutralText;
    public TextConfig effectiveText;
    public TextConfig ineffectiveText;

    //Animation
    [Space()]
    public float textLifeTime = 2f;
    public AnimationCurve verticalMovement = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Gradient colourOverLifeTime;

    private void Awake()
    {
        instance = this;
    }

	public static Effectiveness CalculateEffectiveness(int originalDamage, int newDamage)
	{
		if (originalDamage == newDamage)
			return Effectiveness.Nuetral;
		else
			return newDamage > originalDamage ? Effectiveness.Effective : Effectiveness.Ineffective;
	}

    public void ShowDamageText(Vector2 characterPos, int amount, Effectiveness effectiveNess)
    {
        //Get damage text and make sure it is parented to this canvas
        GameObject obj = ObjectPooler.GetPooledObject(textPrefab);
        obj.transform.SetParent(transform, false);

        //Set the world position, so that it stays there on the canvas
        KeepWorldPosOnCanvas posKeeper = obj.GetComponent<KeepWorldPosOnCanvas>();
        posKeeper.worldPos = characterPos;

        //Set damage text
        Text damageText = obj.GetComponent<Text>();
        Outline outline = obj.GetComponent<Outline>();

        TextConfig config;

        switch(effectiveNess)
		{
			case Effectiveness.Effective:
				config = effectiveText;
				break;

			case Effectiveness.Ineffective:
				config = ineffectiveText;
				break;

			default:
				config = neutralText;
				break;
		}

        if (damageText)
        {
            damageText.text = amount.ToString();
            damageText.fontSize = config.fontSize;
            damageText.color = config.textColor;
        }

        if(outline)
        {
            outline.effectColor = config.outlineColor;
        }

        StartCoroutine("AnimateText", obj);
    }

    IEnumerator AnimateText(GameObject textObject)
    {
        float lifeTime = 0;

        KeepWorldPosOnCanvas posKeeper = textObject.GetComponent<KeepWorldPosOnCanvas>();
        Vector2 initialPos = posKeeper.worldPos;
        Text text = textObject.GetComponent<Text>();

        Color initialColour = text.color;

        while (lifeTime < textLifeTime)
        {
            yield return new WaitForEndOfFrame();
            lifeTime += Time.deltaTime;

            float percent = lifeTime / textLifeTime;

            posKeeper.worldPos = initialPos + new Vector2(0, verticalMovement.Evaluate(percent));
            text.color = initialColour * colourOverLifeTime.Evaluate(percent);
        }

        textObject.SetActive(false);
    }
}
