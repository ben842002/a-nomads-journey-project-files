using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Text versionText;

    LoadLevel level;
    PlayerStats stats;

    [SerializeField] GameObject continueSaveBUTTON;
    [SerializeField] GameObject playBUTTON;
    [SerializeField] GameObject restartButton;
    bool clickedContinue = false;

    // Start is called before the first frame update
    void Start()
    {
        level = LoadLevel.instance;
        stats = PlayerStats.instance;

        // uncomment this to reset for testing purposes (destroys CONTINUE button)
        //PlayerPrefs.SetInt("ContinueSave", 0);

        // first time player boots up game, delete continue save button. Once player saves, delete play button from now on
        int number = PlayerPrefs.GetInt("ContinueSave", 0);
        if (number == 0)
            Destroy(continueSaveBUTTON);
        else
            Destroy(playBUTTON);

        versionText.text = Application.version;
        Cursor.visible = true;

        // destroy restart button until player completes game (reaches thank you screen)
        if (PlayerPrefs.GetInt("Restart") != 1)
            Destroy(restartButton);
    }

    public void Play()
    {
        StartCoroutine(level.LoadLevelIndex(1));
    }

    public void Restart()
    {
        PlayerPrefs.DeleteAll();

        SaveSystem.DeletePlayerFile();
        SaveSystem.DeletePlayerStatsFile();

        // we need to close the game so the PLAY button can be shown correctly in the menu screen
        Application.Quit();
    }

    public void ContinueSave()
    {   
        // avoid repeated calls (generates trash)
        if (clickedContinue == false)
        {
            clickedContinue = true;
            LoadSaveBool.instance.LoadSave = true;

            // load correct level index and saved playerstats values
            PlayerStatsData data = SaveSystem.LoadPlayerStats();

            // all player stats values except levelIndex are referenced below
            GetPlayerStatsData(data);

            // load lvl
            int levelIndex = data.levelIndex;
            StartCoroutine(LoadLevel.instance.LoadLevelIndex(levelIndex));
        }
    }

    // LEVEL INDEX IS NOT INCLUDED!
    void GetPlayerStatsData(PlayerStatsData data)
    {   
        // ADD MORE STAT VARIABLES HERE

        // hp/lives
        stats.maxHealth = data.maxHealth;
        stats.maxLives = data.maxLives;

        // mana
        stats.maxMana = data.maxMana;
        stats.enemyDeathMana = data.enemyDeathMana;

        // abilities
        stats.heal = data.heal;
        stats.dashAbility = data.dashAbility;
        stats.doubleJump = data.doubleJump;
        stats.bowAbility = data.bowAbility;

        // movement
        stats.moveSpeed = data.moveSpeed;
        stats.jumpSpeed = data.jumpSpeed;
        stats.dashSpeed = data.dashSpeed;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("You quit the game!");
    }

    public void OnMouseEnter()
    {
        AudioManager.instance.Play("MouseOver");
    }

    public void MouseClick()
    {
        AudioManager.instance.Play("MouseClick");
    }
}
