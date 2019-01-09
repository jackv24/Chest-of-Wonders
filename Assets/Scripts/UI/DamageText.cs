using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [System.Serializable]
    private struct TextConfig
    {
        public Color textColor;
        public Color outlineColor;
        public int fontSize;
    }

    //Static instance as this should only be attached to one canvas
    private static DamageText instance;

    [SerializeField]
    private GameObject textPrefab;

    [SerializeField]
    private TextConfig neutralText;

    [SerializeField]
    private TextConfig effectiveText;

    [SerializeField]
    private TextConfig ineffectiveText;

    [Header("Animation")]
    [Space, SerializeField]
    private float textLifeTime = 2f;

    [SerializeField]
    private AnimationCurve verticalMovement = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    [SerializeField]
    private Gradient colourOverLifeTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("There should not be more than one DamageText instance!", this);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void ShowDamageText(Vector2 characterPos, int amount, ElementManager.Effectiveness effectiveness)
    {
        if (!instance)
            return;

        instance.InternalShowDamageText(characterPos, amount, effectiveness);
    }

    private void InternalShowDamageText(Vector2 characterPos, int amount, ElementManager.Effectiveness effectiveness)
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

        switch(effectiveness)
		{
			case ElementManager.Effectiveness.Effective:
				config = effectiveText;
				break;

			case ElementManager.Effectiveness.Ineffective:
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

    private IEnumerator AnimateText(GameObject textObject)
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
