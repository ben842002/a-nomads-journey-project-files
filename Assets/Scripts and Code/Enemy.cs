using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    /* 
     * KEY THING TO NOTE ABOUT System.Serializable - What it basically does in layman terms is that
     * it INHERENTLY CREATES AN INSTANCE of a custom class for us. This means that we DO NOT NEED TO code in 
     * inheritance for a variable of that class type. (aka - WE DO NOT NEED TO CODE: 
     *      public EnemyStats stats = new EnemyStats();
     * WE CAN LEAVE IT AS public EnemyStats stats; BECAUSE SYSTEM.SERIALIZABLE DOES THE new EnemyStats(); BASICALLY.
     * IN ADDITION: System.Serialize ALSO lets us edit values in the INSPECTOR while hardcoding
     * inheritance doesn't. So in general, use System.Serialize for Unity
     * for custom classes (we already use it for the Sound script). Only use ClassA a = new ClassA(); when we do not
     * want to change any values in the script. 
     */
    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth = 100;

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public EnemyStats enemyStats;
    PlayerStats stats;
    ManaBar manaBar;

    public HealthBar enemyHealthBar;
    public Animator animator;
    public bool isInvulnerable = false;

    public int healAmount = 2;

    [Header("Destroy Parent Object: DONT ADD IF HAS isDead")]
    public GameObject destroyObject;

    bool hasIsHurt = false;

    private void Start()
    {
        stats = PlayerStats.instance;
        manaBar = GameObject.FindGameObjectWithTag("PlayerManabar").GetComponent<ManaBar>();

        enemyStats.Init();
        
        // initialize health bar values
        enemyHealthBar.SetMaxHealth(enemyStats.maxHealth);
        enemyHealthBar.SetHealth(enemyStats.maxHealth);

        InvokeRepeating(nameof(HealEnemy), 2f, 2f);

        // check if hurt parameter exists
        hasIsHurt = GameMaster.gm.FindParameter("Hurt", animator);
    }

    // function is called from other scripts when this enemy takes damage
    public void DamageEnemy(int damage)
    {   
        if (isInvulnerable == false)
        {
            // take dmg
            enemyStats.currentHealth -= damage;

            // update health bar
            enemyHealthBar.SetHealth(enemyStats.currentHealth);

            // enemy dies when <= 0
            if (enemyStats.currentHealth <= 0)
            {
                AudioManager.instance.Play("EnemyDeath");

                // gain back mana when killing an enemy
                GainManaBack();

                // check if there is a death animation
                bool hasIsDead = GameMaster.gm.FindParameter("isDead", animator);
                if (hasIsDead == true)
                    animator.SetBool("isDead", true);
                else
                    GameMaster.KillEnemy(this);
            }

            if (hasIsHurt == true)
                animator.SetBool("Hurt", true);
        }
    }

    void GainManaBack()
    {
        // gain back mana when killing an enemy
        if (stats.currentMana + stats.enemyDeathMana <= stats.maxMana)
        {
            stats.currentMana += stats.enemyDeathMana;
            manaBar.SetMana(stats.currentMana);
        }
    }

    private void HealEnemy()
    {
        if (enemyStats.currentHealth < enemyStats.maxHealth)
        {
            enemyStats.currentHealth += healAmount;
            enemyHealthBar.SetHealth(enemyStats.currentHealth);
        }
    }

    // when player kills enemy it destroys the parent object
    private void OnDestroy()
    {
        Destroy(destroyObject);
    }
}
