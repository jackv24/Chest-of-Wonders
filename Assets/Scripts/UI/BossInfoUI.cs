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
            //Only update slider if health value has changed
            if(healthSlider && stats.currentHealth != oldHealth)
            {
                float value = stats.currentHealth / (float)stats.maxHealth;

                //Lerp slider value based on speed
                healthSlider.value = Mathf.Lerp(healthSlider.value, value, sliderLerpSpeed * Time.deltaTime);

                //Lerp may not reach zero quickly
                if (Mathf.Abs(healthSlider.value - value) < 0.0001f)
                {
                    //Stop updating slider
                    oldHealth = stats.currentHealth;
                }
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
