using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    bool isInRange;

    [Header("Set this if you want the player to press a button to go to a/next level")]
    [SerializeField]
    private bool inputForNextLevel;
    [SerializeField]
    private bool isAstring = false;
    [SerializeField]
    private string levelName;

    // Update is called once per frame
    void Update()
    {
        if (isInRange == true)
        {   
            // check if the player needs to input in order to go to next lvl
            if (inputForNextLevel == true)
                CheckForPlayerInput();
            else
                LoadByIndex();  // load next lvl (+1 index) automatically once player enters trigger zone
        }
    }

    void CheckForPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {   
            // if we want to load lvl by string
            if (isAstring == true)
            {
                StartCoroutine(LoadLevel.instance.LoadLevelString(levelName));
            }
            else
            {
                // load by index
                LoadByIndex();
            }
        }
    }

    void LoadByIndex()
    {
        // load by index
        int nextLvlIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadLevel.instance.LoadLevelIndex(nextLvlIndex));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isInRange = false;
    }
}
