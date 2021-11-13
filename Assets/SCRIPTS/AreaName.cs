using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaName : MonoBehaviour
{   
    /* The point of the script is to only show the area name at a certain location ONCE. After that,
     * show name immediately in scene. (triggered in start method)
     */

    [SerializeField] GameObject areaName;
    [SerializeField] string playerPrefsName;
    bool triggeredName;

    private void Awake()
    {
        // uncomment this to reset the playerprefs
        //PlayerPrefs.SetInt(playerPrefsName, 0);

        // check if player has already discovered location, and if so, immediately show area name
        int number = PlayerPrefs.GetInt(playerPrefsName, 0);
        if (number == 1)
        {
            Destroy(gameObject);
            GameMaster.gm.triggerOnStartMethod = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && triggeredName == false)
        {
            areaName.SetActive(true);
            triggeredName = true;
            PlayerPrefs.SetInt(playerPrefsName, 1);
        }
    }
}
