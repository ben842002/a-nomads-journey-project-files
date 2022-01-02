using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCombatAbilities : MonoBehaviour
{
    PlayerStats stats;
    Player player;

    HealthBar healthBar;
    ManaBar manaBar;

    [Header("Health Regen Ability: Don't change first var")]
    public int healthAmount;
    [Tooltip("healthAmount will be percentage of player's max hp. KEEP AS A DECIMAL")]
    public float maxHealthPercentage;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;
        player = GetComponent<Player>();

        if (healthBar == null)
            healthBar = GameObject.FindGameObjectWithTag("PlayerHealthbar").GetComponent<HealthBar>();

        if (manaBar == null)
            manaBar = GameObject.FindGameObjectWithTag("PlayerManabar").GetComponent<ManaBar>();

        // make health amount relative to player max hp
        healthAmount = Mathf.RoundToInt(stats.maxHealth * maxHealthPercentage);
    }

    // Update is called once per frame
    void Update()
    {   
        // player heal ability
        if (Input.GetKeyDown(KeyCode.Alpha1) && stats.heal == true)
        {
            if (stats.currentHealth < stats.maxHealth && stats.currentMana >= stats.healManaCost)
                Heal();
        }
    }

    void Heal()
    {
        player.HealPlayer(healthAmount);
        ManaCost(stats.healManaCost);
    }

    void ManaCost(int manaCost)
    {
        stats.currentMana -= manaCost;
        manaBar.SetMana(stats.currentMana);
    }
}
