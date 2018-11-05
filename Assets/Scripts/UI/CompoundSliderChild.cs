using UnityEngine;
using UnityEngine.UI;

public class CompoundSliderChild : MonoBehaviour
{
    public float Value
    {
        get { return fillImage?.fillAmount ?? 0; }
        set { if (fillImage) fillImage.fillAmount = value; }
    }

    [SerializeField]
    private Image fillImage;
}
