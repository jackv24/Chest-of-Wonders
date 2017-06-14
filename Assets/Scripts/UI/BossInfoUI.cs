using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUI : MonoBehaviour
{
    public static BossInfoUI Instance;

    public CharacterStats stats;
    private int oldHealth = 0;

    [Space()]
    public Text nameText;
    public Slider healthSlider;
    public float sliderLerpSpeed = 10.0f;

    void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (stats)
        {
            oldHealth = 0;

            if(nameText)
                nameText.text = stats.gameObject.name;
        }

        if (healthSlider)
            healthSlider.value = 0;
    }

    void Update()
    {
        if(stats)
        {
            if(healthSlider && stats.currentHealth != oldHealth)
            {
                float value = stats.currentHealth / (float)stats.maxHealth;

                healthSlider.value = Mathf.Lerp(healthSlider.value, value, sliderLerpSpeed * Time.deltaTime);

                if (Mathf.Abs(healthSlider.value - value) < 0.001f)
                    oldHealth = stats.currentHealth;
            }
        }
    }

    public void Show(CharacterStats characterStats)
    {
        stats = characterStats;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
