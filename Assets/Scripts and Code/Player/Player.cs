using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    GameMaster gm;
    PlayerStats stats;
    CinemachineShake shake;
    PlayerCombat combat;

    [Header("isVulnerable State")]
    public bool isInvulnerable = false;
    public float invulTimerCountdown = 1f;
    float invulTimer;

    [Header("Health Regen Interval")]
    public float healthRegenRate = 5f;

    [Header("Health and Text")]
    [SerializeField]
    private HealthBar healthBar;  
    [SerializeField]
    private Text healthText;

    [Header("Camera Shake from Damage")]
    public float camIntensity = 10f;
    public float camTime = .15f;

    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameMaster.gm;
        stats = PlayerStats.instance;
        shake = CinemachineShake.instance;
        combat = GetComponent<PlayerCombat>();

        if (healthBar == null)      
            healthBar = GameObject.FindGameObjectWithTag("PlayerHealthbar").GetComponent<HealthBar>();    

        if (healthText == null)
            healthText = GameObject.FindGameObjectWithTag("PlayerHealthtext").GetComponent<Text>();

        // initial health values
        stats.currentHealth = stats.maxHealth;
        healthBar.SetMaxHealth(stats.currentHealth);
        healthBar.SetHealth(stats.currentHealth);
        healthText.text = stats.currentHealth.ToString();

        // player hp regen
        InvokeRepeating(nameof(HealthRegeneration), healthRegenRate, healthRegenRate);

        invulTimer = invulTimerCountdown;
    }

    // Update is called once per frame
    void Update()
    {
        // isInvulnerable timer
        if (isInvulnerable == true)
        {
            if (invulTimer <= 0)
            {
                isInvulnerable = false;
                invulTimer = invulTimerCountdown;
            }
            else
                invulTimer -= Time.deltaTime;
        }

        if (isDead == true)
        {
            GameMaster.KillPlayer(this);
            isDead = false;
            return;
        }

        if (transform.position.y <= gm.fallBoundary)
            DamagePlayer(9999);
    }

    public void DamagePlayer(int damage)
    {   
        if (isInvulnerable == false)
        {
            // let player take no damage for a brief amount of time after taking dmg
            // probably use this as an ability
            //isInvulnerable = true;

            // cancel combo attack
            if (combat.combo == true)
            {
                combat.combo = false;
                GetComponent<Animator>().SetBool("RegularCombo", false);
            }
            else if (combat.airCombo == true)
            {
                combat.airCombo = false;
                GetComponent<Animator>().SetBool("AirCombo", false);
            }

            // take dmg
            stats.currentHealth -= damage;

            // health bar and text
            healthBar.SetHealth(stats.currentHealth);
            healthText.text = stats.currentHealth.ToString();

            if (stats.currentHealth <= 0)
            {
                // kill player
                isDead = true;
            }
            else
            {   
                AudioManager.instance.Play("PlayerHit");

                // camera shake
                shake.ShakeCamera(camIntensity, camTime);
            }
        }
    }

    /// <summary>
    /// call this function when the player gains health.
    /// Could be called in player abilities or when player kills a certain enemy
    /// </summary>
    public void HealPlayer(int healthAmount)
    {
        stats.currentHealth += healthAmount;

        healthBar.SetHealth(stats.currentHealth);
        healthText.text = stats.currentHealth.ToString();
    }

    void HealthRegeneration()
    {
        stats.currentHealth += stats.healthRegenAmount;

        healthBar.SetHealth(stats.currentHealth);
        healthText.text = stats.currentHealth.ToString();
    }
}
