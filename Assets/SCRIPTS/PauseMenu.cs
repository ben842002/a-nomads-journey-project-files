using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; // Can reference this to other scripts 
                                             // Used in Weapon and movement script
    public GameObject pauseMenuUI;
    public GameObject settingsUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused == true)
            {
                // if the game is paused and player presses ESC, call the resume function
                Resume();
            }
            else
            {
                // if the game isn't paused, pause the game and set pause menu active
                Pause();
            }
        }
    }

    public void Resume()
    {
        // When the player is in settings mode and presses esc, return to pause menu instead of unpausing game
        if (settingsUI.activeSelf == true)
        {
            pauseMenuUI.SetActive(true);
            settingsUI.SetActive(false);
        }
        // Unpause the game if player is in the pause menu
        else
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; // resumes time
            GameIsPaused = false;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // freeze time/game
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        // resume time 
        Time.timeScale = 1f;

        // make cross fade canvas in front of others
        GameObject.FindGameObjectWithTag("CrossFade").GetComponent<Canvas>().sortingOrder = 10;

        // since time is resumed, destroy player so that player cant move around or die while transitioning
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);

        // if coroutine doesnt load, make sure scene in loaded in build settings
        StartCoroutine(LoadLevel.instance.LoadLevelIndex(0));
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
