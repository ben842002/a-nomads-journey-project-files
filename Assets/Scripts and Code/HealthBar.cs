using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    /// <summary>
    /// function is called on start AND when max values (aka stats.maxHealth or mana is increased)
    /// </summary>
    public void SetMaxHealth(int health)
    {
        // make maxValue equal to parameter int
        slider.maxValue = health;  

        // adjust color in editor
        fill.color = gradient.Evaluate(1f);
    }
    /// <summary>
    /// this function is called after damage has been applied or current health has been increased
    /// </summary>
    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
