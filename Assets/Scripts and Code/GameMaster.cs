using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    PlayerStats stats;
    CinemachineShake shake;

    [Header("Fall Bounday and Respawn")]
    public float fallBoundary;
    public float respawnDelay;
    public GameObject playerPrefab;
    public Vector2 checkPointPosition;

    [Header("Death Camera Shake")]
    public float camIntensity = 15f;
    public float camTime = 0.2f;

    [Header("Cinemachine Camera Array")]
    public CinemachineVirtualCamera[] cinemachineCameras;

    [Header("UI")]
    public GameObject minusLivesText;
    public GameObject gameOverUI;
    public GameObject settingsMenu;
    public GameObject pauseMenu;
    public GameObject saveICON;

    [Header("Generic Enemy Death Prefab")]
    public GameObject enemyDeathPrefab;

    [Header("Area/Biome Name")]
    public bool triggerOnStartMethod = true;
    public GameObject areaNameUI;

    ManaBar manaBar;

    bool initalizeLivesText;

    private void OnLevelWasLoaded()
    {   
        if (LoadSaveBool.instance != null)
        {
            if (LoadSaveBool.instance.LoadSave == true)
            {
                // load player position (stats values were loaded from menu button call)
                PlayerData data = SaveSystem.LoadPlayer();

                // find player in scene
                Transform player = GameObject.FindGameObjectWithTag("Player").transform;

                // get cord values and apply to player transform
                Vector3 savedPosition;
                savedPosition.x = data.position[0];
                savedPosition.y = data.position[1];
                savedPosition.z = data.position[2];
                player.position = savedPosition;

                // reset bool
                LoadSaveBool.instance.LoadSave = false;
            }
        }
    }

    private void Awake()
    {
        if (gm == null)
            gm = this;

        // -1 for reset
        //Application.targetFrameRate = -1;
    }

    private void Start()
    {
        stats = PlayerStats.instance;
        shake = CinemachineShake.instance;

        if (triggerOnStartMethod == true)
        {
            if (areaNameUI.activeSelf == false)
                areaNameUI.SetActive(true);
        }

        // add all cinemachine cameras to the list
        cinemachineCameras = FindObjectsOfType<CinemachineVirtualCamera>();

        // find mana bar
        manaBar = GameObject.FindGameObjectWithTag("PlayerManabar").GetComponent<ManaBar>();

        SaveSystem.SavePlayer(FindObjectOfType<Player>());
        SaveSystem.SavePlayerStats(FindObjectOfType<PlayerStats>());
        CurrentAndMaxLives.instance.UpdateLives();
        PlayerPrefs.SetInt("ContinueSave", 1);

        //saveICON.SetActive(true); // for now, do not play the save icon at beginning of lvl

        // un(comment) to keep/delete files
        //SaveSystem.DeletePlayerFile();
        //SaveSystem.DeletePlayerStatsFile();
    }

    private void Update()
    {   
        // PlayerStats OnlevelwasLoaded function resets current lives to max lives. However, updatelives() does not work
        // in either Start() or OnLevelwasLoaded() so it is placed here (GM object is unique per scene)
        if (initalizeLivesText == true)
        {
            CurrentAndMaxLives.instance.UpdateLives();
            initalizeLivesText = false;
        }

        if (gameOverUI.activeSelf == true || pauseMenu.activeSelf == true || settingsMenu.activeSelf == true)
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }

    // store object for death anim logic purposes
    GameObject player;

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnDelay);
        Destroy(player);

        // spawn player after delay
        GameObject _player = Instantiate(playerPrefab, checkPointPosition, Quaternion.identity);

        // make cameras follow player again
        for (int i = 0; i < cinemachineCameras.Length; i++)
            cinemachineCameras[i].m_Follow = _player.transform;
    }

    void EndGame()
    {
        AudioManager.instance.Play("GameOver");
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// Kill functions are below. The structure is that scripts should call GameMaster.Kill___.
    /// The actual code/logic should be located in the private void such as _KillPlayer
    /// </summary>
    public static void KillPlayer(Player player)
    {
        gm._KillPlayer(player);
    }

    // this function is called through KillPlayer
    private void _KillPlayer(Player _player)
    {   
        // store _player gameobject into var player to be destroyed in RespawnPlayer();
        player = _player.gameObject;

        // shake camera, then reset cameras in 1 second
        shake.ShakeCamera(camIntensity, camTime);
        Invoke(nameof(ResetCameraAmpAndFreq), 1f);

        // reset velocity before possibly deleting component
        _player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // disable player components when player dies (Two situations: dies in air, dies on ground)
        if (_player.GetComponent<PlayerMovement>().isGrounded == true)
        {
            Destroy(_player.GetComponent<Rigidbody2D>());
            Destroy(_player.GetComponent<Collider2D>());
            DisablePlayer(_player);
        }
        else
        {   
            // dont disable rb and collider
            DisablePlayer(_player);
        }

        // subtract lives
        stats.currentLives -= 1;

        // update lives text and play sound
        CurrentAndMaxLives.instance.UpdateLives();
        minusLivesText.SetActive(true);
        AudioManager.instance.Play("PlayerDeath");

        if (stats.currentLives <= 0)
            EndGame();
        else
            gm.StartCoroutine(gm.RespawnPlayer());
    }

    // function to reset amp and freq (avoid cam shake bugs)
    void ResetCameraAmpAndFreq()
    {
        for (int i = 0; i < cinemachineCameras.Length; i++)
            shake.ResetValues(cinemachineCameras[i]);
    }

    void DisablePlayer(Player _player)
    {
        // change layer and tag so player isnt targeted by enemies
        _player.gameObject.layer = 0;
        _player.tag = "Untagged";

        // access components
        Animator anim = _player.GetComponent<Animator>();
        PlayerMovement move = _player.GetComponent<PlayerMovement>();

        // bug where player dash still shows anim
        if (anim.GetBool("isDashing") == true)
        {
            anim.SetBool("isDashing", false);
            move.isDashing = false;
        }

        // make sure player does flip out when dying (mass is 1, so any collision will move object fast)
        if (_player.TryGetComponent<Rigidbody2D>(out _))
            _player.GetComponent<Rigidbody2D>().mass = 999;

        _player.GetComponent<Animator>().SetBool("isDead", true);
        Destroy(_player.GetComponent<Player>());
        Destroy(move);
        Destroy(_player.GetComponent<PlayerCombat>());
        Destroy(_player.GetComponent<NonCombatAbilities>());
        Destroy(_player.GetComponent<Platform>());
    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    private void _KillEnemy(Enemy _enemy)
    {
        // spawn effect if there is no death anim for that specific enemy
        GameObject effect = Instantiate(enemyDeathPrefab, _enemy.transform.position, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(_enemy.gameObject);
    }
    
    // ---------------------------------------------------------

    public bool FindParameter(string paramName, Animator animator)
    {
        // loop through each parameter in the animator parameter list
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (paramName == parameter.name)
                return true;

        return false;
    }
}
