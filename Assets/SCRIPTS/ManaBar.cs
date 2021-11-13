using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    /// <summary>
    /// function is called on start AND when max values (aka stats.maxMana is increased)
    /// </summary>
    public void SetMaxMana(int mana)
    {
        // make maxValue equal to parameter int
        slider.maxValue = mana;

        // adjust color in editor
        fill.color = gradient.Evaluate(1f);
    }

    // this function is called after mana has been used
    public void SetMana(int mana)
    {
        slider.value = mana;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
