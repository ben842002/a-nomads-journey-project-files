using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField] GameObject textSaveUI;
    [SerializeField] GameObject textSaveICON;
    bool isInRange;

    private void Start()
    {   
        if (textSaveUI.activeSelf == true)
            textSaveUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isInRange == true)
        {
            // save data
            Save();

            // reset lives
            PlayerStats.instance.currentLives = PlayerStats.instance.maxLives;
            CurrentAndMaxLives.instance.UpdateLives();

            // show continue button in menu
            if (PlayerPrefs.GetInt("ContinueSave") == 0)
                PlayerPrefs.SetInt("ContinueSave", 1);
        }
    }

    void Save()
    {   
        // indicator to player data has been saved on PlayerCanvas
        textSaveICON.SetActive(true);

        SaveSystem.SavePlayer(FindObjectOfType<Player>());
        SaveSystem.SavePlayerStats(FindObjectOfType<PlayerStats>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            textSaveUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            textSaveUI.SetActive(false);
        }
    }
}
