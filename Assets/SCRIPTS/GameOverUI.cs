using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("You quit the game.");
    }

    public void Restart()
    {
        LoadSaveBool.instance.LoadSave = true;

        // load current scene again
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadLevel.instance.LoadLevelIndex(currentLevelIndex));
    }
}
