using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentAndMaxLives : MonoBehaviour
{
    public static CurrentAndMaxLives instance;

    [SerializeField] Text currentLivesText;
    [SerializeField] Text maxLivesText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
    }

    public void UpdateLives()
    {
        currentLivesText.text = ": " + PlayerStats.instance.currentLives;
        maxLivesText.text = "/" + PlayerStats.instance.maxLives;
    }
}
