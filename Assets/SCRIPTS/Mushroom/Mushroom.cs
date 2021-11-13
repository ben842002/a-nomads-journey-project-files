using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Animator anim;
     
    public bool facingRight = true;
    [SerializeField] float moveSpeed;
    [SerializeField] int damage;
    [SerializeField] float attackRate;
    float nextAttack;

    [SerializeField] float colDamageTimerC;
    float colDamageTimer;
    bool colDmg;

    [Header("Player In Range Check")]
    [SerializeField] bool autoRunToPlayer;
    [SerializeField] float playerCheckRadius;

    [Header("isGrounded")]
    public bool isGrounded;
    [SerializeField] float groundRaycast;
    [SerializeField] LayerMask groundMask;

    [Header("Attack")]
    public bool playerInRange;  // check is called in CheckForPlayerAttack script
    [SerializeField] Transform Attack1Point;
    [SerializeField] Transform Attack2Point;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float raycastDistance;

    [Header("Projectile Components")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projTimerCountdown;
    float projTimer;

    [Header("OPTIONAL: Boss fight")]
    public GameObject ambushScript;

    [HideInInspector] public Transform player;
    float nextSearch;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        projTimer = projTimerCountdown;
        colDamageTimer = colDamageTimerC;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // check if on ground
        isGrounded = Physics2D.Raycast(transform.parent.position, Vector2.down, groundRaycast, groundMask);

        // check if player in range
        bool playerInRange = Physics2D.OverlapCircle(transform.parent.position, playerCheckRadius, playerMask);
        if (playerInRange == true)
            SpawnProjectileTimer();

        // autoRunToPlayer makes the bool true regardless of player proximity
        if (autoRunToPlayer == false)
        {
            // check if player is in range
            if (playerInRange == true)
                anim.SetBool("Run", true);
            else
                anim.SetBool("Run", false);
        }
        else
            anim.SetBool("Run", true);

        // collision damage
        if (colDmg == true)
            CollisionDamageTimer();
    }

    void SpawnProjectileTimer()
    {
        if (projTimer <= 0)
        {
            anim.SetTrigger("Projectile");
            AudioManager.instance.Play("MushroomSummon");

            // randomize timercountdown a bit 
            float timerC = Random.Range(projTimerCountdown - 1, projTimerCountdown + 1);
            projTimer = timerC;
        }
        else
            projTimer -= Time.deltaTime;
    }

    void CollisionDamageTimer()
    {
        if (colDamageTimer <= 0)
        {
            player.GetComponent<Player>().DamagePlayer(damage / 2);
            colDamageTimer = colDamageTimerC;
        }
        else
            colDamageTimer -= Time.deltaTime;
    }

    // called on projectile animation frame
    void SpawnProjectile()
    {
        float upperExtent = transform.position.y + 6f;
        float bottomExtent = transform.position.y - 6f;
        float leftExtent = transform.position.x - 6f;
        float rightExtent = transform.position.x + 6f;

        // find random position to spawn
        float randomX = Random.Range(leftExtent, rightExtent);
        float randomY = Random.Range(bottomExtent, upperExtent);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
    }

    // called on mushroom_run anim script
    // check if player is in range for attack
    public bool CheckForPlayer(Vector2 dir)
    {
        bool playerInRange = Physics2D.Raycast(transform.parent.position, dir, raycastDistance, playerMask);
        return playerInRange;
    }

    // called on mushroom_run anim script
    public void Attack()
    {
        if (nextAttack <= Time.time)
        {
            // choose randomly between two attacks
            int random = Random.Range(0, 2);
            if (random == 0)
                anim.SetTrigger("Attack1");
            else
                anim.SetTrigger("Attack2");

            nextAttack = Time.time + 1f / attackRate;
        }
    }

    // called in mushroom_run script
    public void FaceObject(Vector2 targetPosition)
    {
        if (targetPosition.x > transform.parent.position.x)
        {
            transform.localScale = new Vector2(1, 1);
            facingRight = true;
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
            facingRight = false;
        }
    }

    // called in animaation behavior scripts
    public void MoveTowardsObject(Vector2 objectLocation)
    {
        Vector2 objectLocationX = new Vector2(objectLocation.x, transform.parent.position.y);
        transform.parent.position = Vector2.MoveTowards(transform.parent.position, objectLocationX, moveSpeed * Time.deltaTime);
    }

    // ------------------------------------------------------------
    // functions are called on attack anim frame except for first
    void DamagePlayer(Collider2D player)
    {
        player.GetComponent<Player>().DamagePlayer(damage);
        PlayerKnockback.instance.KnockBackPlayer(player.GetComponent<Collider2D>(), transform.parent.gameObject);
    }

    void Attack1()
    {
        AudioManager.instance.Play("MushroomBite");
        Collider2D player = Physics2D.OverlapCircle(Attack1Point.position, attackRadius, playerMask);
        if (player != null)
            DamagePlayer(player);
    }

    void Attack2()
    {
        AudioManager.instance.Play("MushroomBite");
        Collider2D player = Physics2D.OverlapCircle(Attack2Point.position, attackRadius, playerMask);
        if (player != null)
            DamagePlayer(player);
    }

    // ------------------------------------------------------------

    void FindPlayer()
    {
        if (nextSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextSearch = Time.time + 1f;
        }
    }

    // --------------------------------
    // ONCOLLISION CODE
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
            DamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            colDmg = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            colDmg = false;
            colDamageTimer = colDamageTimerC;
        }
    }

    // --------------------------------

    private void OnDrawGizmosSelected()
    {   
        // player detection
        Gizmos.DrawWireSphere(transform.parent.position, playerCheckRadius);

        // groundcheck and player in range for attack
        Gizmos.DrawRay(transform.parent.position, Vector2.down * groundRaycast);
        Gizmos.DrawRay(transform.parent.position, Vector2.right * raycastDistance);

        // attack colliders
        Gizmos.DrawWireSphere(Attack1Point.position, attackRadius);
        Gizmos.DrawWireSphere(Attack2Point.position, attackRadius);
    }
}
