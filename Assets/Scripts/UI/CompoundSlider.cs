using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class CompoundSlider : MonoBehaviour
{
    public float Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public int BarCount
    {
        get
        {
            return childSliders
            .Where(child => child != null && child.gameObject.activeSelf)
            .Count();
        }
        set
        {
            for (int i = 0; i < childSliders.Length; i++)
            {
                if (!childSliders[i])
                    continue;

                childSliders[i].gameObject.SetActive(i < value);
            }
        }
    }

    [SerializeField, Range(0, 1.0f)]
    private float value;

    [SerializeField]
    private CompoundSliderChild[] childSliders;

    private void Update()
    {
        UpdateChildSliderValues();
    }

    private void UpdateChildSliderValues()
    {
        if (childSliders == null)
            return;

        int count = BarCount;

        if (count <= 0)
            return;

        float slidersFill = count * value;
        int fullSliders = (int)slidersFill;
        float remainder = slidersFill % 1;

        for (int i = 0; i < count; i++)
        {
            if (!childSliders[i])
                continue;

            float fillAmount;
            if (i < fullSliders)
                fillAmount = 1.0f;
            else if (i == fullSliders)
                fillAmount = remainder;
            else
                fillAmount = 0;

            childSliders[i].Value = fillAmount;
        }
    }
}
