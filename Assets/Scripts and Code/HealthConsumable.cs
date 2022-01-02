using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthConsumable : MonoBehaviour
{
    [SerializeField] int healthAmount;
    [SerializeField] GameObject itemFeedback;

    [Header("Rename this for EVERY HP item! (if maxLives bool is triggered)")]
    [SerializeField] bool usePlayerPrefs;
    [SerializeField] string playerPrefsName;    // no two HP item can have the same playerprefs!

    [Header("Lives")]
    public bool addMaxLivesWithCurrent;
    [SerializeField] int currentLivesGained;
    
    PlayerStats stats;
    HealthBar hp;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;
        hp = GameObject.FindGameObjectWithTag("PlayerHealthbar").GetComponent<HealthBar>();

        // comment this to reset the player prefs
        //PlayerPrefs.SetInt(playerPrefsName, 0);

        if (usePlayerPrefs == true)
        {
            int num = PlayerPrefs.GetInt(playerPrefsName, 0);
            if (num == 1)
            {
                // destroy object if player has already picken up item
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            // hp
            stats.currentHealth += healthAmount;
            hp.SetHealth(stats.currentHealth);

            // lives 
            if (currentLivesGained != 0)
            {
                stats.currentLives += currentLivesGained;
                if (addMaxLivesWithCurrent == true)
                {
                    // save if player gets max lives increased
                    stats.maxLives += currentLivesGained;
                    SaveSystem.SavePlayerStats(FindObjectOfType<PlayerStats>());

                    // make sure player cant abuse by reloading and increasing max lives
                    PlayerPrefs.SetInt(playerPrefsName, 1);
                }

                CurrentAndMaxLives.instance.UpdateLives();
            }

            GameObject effect = Instantiate(itemFeedback, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);

            Destroy(gameObject);
        }
    }
}
