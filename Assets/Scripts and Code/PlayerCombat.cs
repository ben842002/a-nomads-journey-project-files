using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{   
    // these bools are used from other scripts
    public bool isAttacking;        // isAttacking is set to true and false in animation scripts attached to anim states
    public bool combo = false;
    public bool airCombo = false;
    public LayerMask enemyMask;

    [Header("Mana Gain per Attack Hit")]
    [SerializeField] int manaGain;

    [Header("Reg. Attack 1")]
    public Transform attackPoint;
    public float attackRadius;
    public Transform attackPoint2;
    public float attackRadius2;
    public Collider2D attack3Collider;

    [Header("Air Attack")]
    public Collider2D airAttack1Collider;
    public Transform airAttackPoint2;
    public float airAttack2Radius;

    [Header("Attack Damage")]
    public int attackDamage;

    [Header("Bow Attack")]
    public Transform arrowSpawnPos;
    public GameObject arrowPrefab;

    [Header("Attack Rates")]
    public float attackRate;
    float nextTimeAttack = 0f;
    public float airAttackRate;
    float nextTimeAirAttack = 0f;
    public float bowAttackRate;
    float nextTimeBowAttack;

    [Header("Hit Effects")]
    [SerializeField] float effectDestroyTime;
    public GameObject tempEffect;

    Animator animator;
    PlayerStats stats;
    PlayerMovement movement;
    ManaBar manaBar;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();

        if (manaBar == null)
            manaBar = GameObject.FindGameObjectWithTag("PlayerManabar").GetComponent<ManaBar>();

        // initialize mana values
        stats.currentMana = stats.maxMana;
        manaBar.SetMaxMana(stats.maxMana);
        manaBar.SetMana(stats.maxMana);
    }

    // Update is called once per frame
    void Update()
    {   
        // layer indices goes TOP (0 index) --> BOTTOM 
        if (isAttacking == false)
            animator.SetLayerWeight(2, 0);

        if (PauseMenu.GameIsPaused == false)
        {
            // dont let player attack if they are knocked back, dashing OR in water
            if (movement.knockBackTimer <= 0 && movement.isInWater == false && movement.isDashing == false)
            {   
                AirAttack();
                RegularAttack();
                BowAndArrow();
            }
        }
    }

    // damage function to reduce code redundancy and increase in ease of processing
    // note: DamageEnemy() is also called from scripts attached to player attack colliders
    public void DamageEnemy(Collider2D enemy, int damage)
    {
        // gain a little mana back when dealing dmg
        if (stats.currentMana < stats.maxMana)
        {   
            stats.currentMana += manaGain;
            if (stats.currentMana > stats.maxMana)
                stats.currentMana = stats.maxMana;

            manaBar.SetMana(stats.currentMana);
        }

        if (enemy.TryGetComponent<Enemy>(out _))
        {
            // sound
            enemy.GetComponent<Enemy>().DamageEnemy(damage);
        }

        // knockback
        if (enemy.TryGetComponent<EnemyKnockback>(out _))
        {   
            // access component
            EnemyKnockback enemyKB = enemy.GetComponent<EnemyKnockback>();

            enemyKB.KnockBackTimer = enemyKB.knockBackTimerCountdown;

            if (transform.position.x > enemy.transform.position.x)
                enemyKB.knockFromRight = true;
            else
                enemyKB.knockFromRight = false;
        }

        AudioManager.instance.Play("EnemyHit");
        GameObject effect = Instantiate(tempEffect, enemy.transform.position, transform.rotation);
        Destroy(effect, 0.5f);
    }

    // use this function if attack can be performed with a circle 
    void AttackPoint(Transform attackPoint, float attackRadius)
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyMask);
        for (int i = 0; i < enemiesHit.Length; i++)
            DamageEnemy(enemiesHit[i], attackDamage);
    }

    // --------------------------------------------------------
    void BowAndArrow()
    {   
        // check if player has ability,has enough mana, AND is not attacking (dont want to play 2 attacks at once)
        if (stats.bowAbility == true && stats.currentMana >= stats.arrowManaCost && isAttacking == false)
        {
            if (Input.GetKeyDown(KeyCode.Q) && nextTimeBowAttack <= Time.time)
            {
                AudioManager.instance.Play("BowRelease");
                animator.SetLayerWeight(2, 1);
                animator.SetBool("Bow", true);

                // attack speed
                nextTimeBowAttack = Time.time + 1f / bowAttackRate;

                // adjust mana stuff
                stats.currentMana -= stats.arrowManaCost;
                manaBar.SetMana(stats.currentMana);
            }
        }
    }

    // called in bow anim frame
    void SpawnArrow()
    {
        Instantiate(arrowPrefab, arrowSpawnPos.position, Quaternion.identity);
    }

    // --------------------------------------------------------
    // GROUND ATTACKS
    void RegularAttack()
    {
        // cancel attack automatically if player jumps in between attack
        // delete if feedback doesn't like it
        if (movement.isGrounded == false && combo == true)
        {
            combo = false;
            animator.SetBool("RegularCombo", false);
        }
            
        // initial attack
        if (Input.GetKeyDown(KeyCode.Mouse0) && nextTimeAttack <= Time.time && movement.isGrounded == true)
        {   
            // addresses bug where if player clicks Mouse0 while after second attack, it plays the first attack anim right after
            // the third attack WHILE playing both sounds at the same time
            if (isAttacking == false)
            {
                animator.SetLayerWeight(2, 1);
                animator.SetTrigger("RegularAttack1");
                AudioManager.instance.Play("Basic Attack 1");
                combo = true;

                nextTimeAttack = Time.time + 1f / attackRate;
            }
        }

        // perform combo if player holds mouse 0
        ComboAttack1();
    }

    void ComboAttack1()
    {   
        if (Input.GetKey(KeyCode.Mouse0) && combo == true)
        {
            animator.SetBool("RegularCombo", true);
        }

        // stop combo is player doesnt hold mouse0
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            combo = false;
            animator.SetBool("RegularCombo", false);
        }
    }

    // -------------------------------------------------------
    // AIR ATTACKS
    void AirAttack()
    {   
        // initial air attack
        if (Input.GetKeyDown(KeyCode.Mouse0) && movement.isGrounded == false && nextTimeAirAttack <= Time.time)
        {
            animator.SetLayerWeight(2, 1);
            animator.SetTrigger("AirAttack1");

            airCombo = true;
            nextTimeAirAttack = Time.time + 1f / airAttackRate;
        }

        // if player holds on mouse0 after initial jump
        AirComboAttack();
    }

    void AirComboAttack()
    {   
        // cancel air combo if player hits the ground
        if (movement.isGrounded == true)
        {
            animator.SetBool("AirCombo", false);
            airCombo = false;
        }
        // perform air combo
        else if (Input.GetKey(KeyCode.Mouse0) && airCombo == true)
            animator.SetBool("AirCombo", true);

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            animator.SetBool("AirCombo", false);
            airCombo = false;
        }
    }

    // --------------------------------------------
    // Enable colliders

    void EnableCombo3Collider()
    {
        attack3Collider.enabled = true;
    }

    void DisableCombo3Collider()
    {
        attack3Collider.enabled = false;
    }

    void EnableAirAttackCollider1()
    {
        airAttack1Collider.enabled = true;
    }

    void DisableAirAttackCollider1()
    {
        airAttack1Collider.enabled = false;
    }

    // ---------------------------------------------
    // The functions below are called on animation frame call to sync with frame
    public void StandardAttack1()
    {
        AttackPoint(attackPoint, attackRadius);
    }

    public void StandardAttack2()
    {
        AttackPoint(attackPoint2, attackRadius2);
    }

    public void AirAttack2()
    {
        AttackPoint(airAttackPoint2, airAttack2Radius);
    }

    // -------------------------------------------
    // additional sounds that are called on anim frame
    void PlayBasicAttack2Sound()
    {
        AudioManager.instance.Play("Basic Attack 2");
    }

    void PlayBasicAttack3Sound()
    {
        AudioManager.instance.Play("Basic Attack 1");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        if (attackPoint2 == null)
            return;

        if (airAttackPoint2 == null)
            return;

        // comment in or out to see specific colliders
        // ------------------------------------------------------------------
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        //Gizmos.DrawWireSphere(attackPoint2.position, attackRadius2);
        //Gizmos.DrawWireSphere(airAttackPoint2.position, airAttack2Radius);
    }
}
