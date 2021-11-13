using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{   
    public bool facingRight = true;
    Animator animator;
    Rigidbody2D rb;

    [SerializeField] int damage;

    [Header("Summon Timer")]
    public float summonTimerC;
    float summonTimer;

    [Header("Movement Values")]
    public float runSpeed;
    public float jumpSpeedX;
    public float jumpSpeedY;

    [Header("Player Check Projectile Attack")]
    public float raycastCheckDistance;
    public bool isInRangeForAttack;
    public LayerMask playerMask;
    public GameObject projectile1Prefab;
    public Transform projectileSpawnPos;
    public float attackRate;
    float nextTimeAttack;

    [Header("Wall Raycast Detection")]
    public float wallRaycheckDistance;
    public bool wallLocated;

    [Header("isGrounded")]
    public Transform groundCheckPos;
    public LayerMask groundMask;
    public float groundCheckRadius;
    public bool isGrounded;

    [Header("Jump Components")]
    public float jumpRaycastDistance;
    public LayerMask wallMask;

    // jump timer to prevent repetitive jumping 
    public float jumpTimerC;    
    float jumpTimer;

    [Header("OnCollisionStay Timer")]
    public float dmgCountdownTimer = 0.5f;
    float dmgTimer;
    public bool doDmg = false;

    [HideInInspector] public Transform player;
    float nextSearch;

    float localScale;

    // Start is called before the first frame update
    void Start()
    {   
        animator = GetComponent<Animator>();
        localScale = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();

        // initialize timer
        summonTimer = summonTimerC;

        dmgTimer = dmgCountdownTimer;
        jumpTimer = jumpTimerC;
    }

    private void Update()
    {   
        // check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, groundMask);

        // summon timer
        if (animator.GetBool("isJumping") == false)
        {
            if (summonTimer <= 0)
            {
                animator.SetLayerWeight(1, 1);
                animator.SetBool("Run", false);
                animator.SetBool("Summon", true);

                summonTimer = summonTimerC;
            }
            else
                summonTimer -= Time.deltaTime;
        }

        // OnCollisonStay damage
        if (doDmg == true)
        {
            if (dmgTimer <= 0)
            {
                player.GetComponent<Player>().DamagePlayer(damage / 2);
                dmgTimer = dmgCountdownTimer;
            }
            else
                dmgTimer -= Time.deltaTime;
        }
    }

    // ---------------------------------------------------------------
    // functions below are called in the animation behaviour scripts

    public void Jump()
    {   
        /* by any chance you have to move this code back into update function in this script,
         * make sure you add an addition child if statement under isGrounded == true checking that the boss isnt attacking
         */

        // check if boss can jump (only if grounded and is not attacking in anyway)
        if (isGrounded == true)
        {
            if (jumpTimer <= 0)
            {
                if (facingRight == true)
                    RaycastCheckForJump(Vector2.right, Color.red);
                else
                    RaycastCheckForJump(Vector2.left, Color.white);

                // reset timer
                jumpTimer = jumpTimerC;
            }
            else
                jumpTimer -= Time.deltaTime;
        }
    }
    
    void RaycastCheckForJump(Vector2 direction, Color color)
    {   
        // check if there are no walls
        if (Physics2D.Raycast(transform.position, direction, jumpRaycastDistance, wallMask) != true)
        {   
            // 50% chance to jump
            int random = Random.Range(0, 2);
            if (random == 1)
            {
                animator.SetBool("isJumping", true);

                if (facingRight)
                    rb.velocity = new Vector2(jumpSpeedX, jumpSpeedY);
                else
                    rb.velocity = new Vector2(-jumpSpeedX, jumpSpeedY);

                Debug.DrawRay(transform.position, direction * jumpRaycastDistance, color, 5f);
            }
        }
    }

    public void CheckForPlayerForAttack(Vector2 direction, Color color)
    {
        isInRangeForAttack = Physics2D.Raycast(transform.position, direction, raycastCheckDistance, playerMask);
        if (isInRangeForAttack == true)
        {   
            if (nextTimeAttack <= Time.time)
            {
                animator.SetLayerWeight(1, 1);
                animator.SetBool("Projectile", true);
                animator.SetBool("Run", false);

                Debug.DrawRay(transform.position, direction * raycastCheckDistance, color, 5f);

                nextTimeAttack = Time.time + 1f / attackRate;
            }
        }
    }

    public void CheckForWalls(Vector2 direction, Color color)
    {
        wallLocated = Physics2D.Raycast(transform.position, direction, wallRaycheckDistance, wallMask);
        Debug.DrawRay(transform.position, direction * wallRaycheckDistance, color, 5f);
    }

    public void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            facingRight = true;
            transform.localScale = new Vector2(localScale, localScale);
        }
        else
        {
            facingRight = false;
            transform.localScale = new Vector2(-localScale, localScale);
        }
    }

    public void FindPlayer()
    {
        if (nextSearch <= Time.time)
        {
            GameObject sResult = GameObject.FindGameObjectWithTag("Player");
            if (sResult != null)
                player = sResult.transform;
            nextSearch = Time.time + 0.75f;
        }
    }

    // called on animation frame
    void SpawnProjectile()
    {
        AudioManager.instance.Play("DeathBasic");
        Instantiate(projectile1Prefab, projectileSpawnPos.position, Quaternion.identity);
    }

    // -------------------------------------------------
    // ONCOLLISION DMG FUNCTIONS

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage / 2);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            doDmg = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            doDmg = false;
            dmgTimer = dmgCountdownTimer;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
    }
}
