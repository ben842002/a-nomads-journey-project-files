using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Level Index for saving")]
    public int levelIndex;

    [Header("Health and lives")]
    public int maxHealth = 100;
    public int currentLives;
    public int maxLives;
    public int healthRegenAmount = 5;

    [Header("Mana")]
    public int maxMana;
    public int currentMana;
    public int enemyDeathMana = 10;

    [Header("Player Learned Abilities?")]
    public bool heal;
    public bool dashAbility;
    public bool doubleJump;
    public bool bowAbility;

    [Header("Player Abilities")]
    public int healManaCost;
    public int arrowManaCost;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float jumpSpeed = 10f;
    public float dashSpeed;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // persist stats between scenes
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded()
    {
        // reset lives
        currentLives = maxLives;

        // get the correct index when loading to another scene
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {   
        // addresses the case where in editor, playing a certain level first will result in index being 0
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // health
    [Header("Dont change! ReadOnly!")]
    [SerializeField] private int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }
}
