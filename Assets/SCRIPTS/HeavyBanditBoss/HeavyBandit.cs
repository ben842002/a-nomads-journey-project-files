using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBandit : MonoBehaviour
{
    public bool facingRight;

    // address one off case where player is against wall and enemy pushes them into it (causes physics bugs)
    [Header("Wall Check: Prevent ramming into wall")]
    [SerializeField] float wallRaycastDistance;
    [SerializeField] LayerMask wallMask;
    public bool stopMoving;

    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    float nextTimeAttack;

    [Header("Jumping Components")]
    // make sure it goes from L -> G for values
    public float raycastDistanceForJump1;
    public float raycastDistanceForJump2;
    public float[] jumpSpeedsX;
    public float[] jumpSpeedsY;
    public float jumpTimerCountdown;
    float jumpTimer;

    [Header("Run Speed")]
    public float runSpeed;

    [Header("Standard Attack")]
    public float raycastDistance;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask playerMask;

    [Header("Ground Check")]
    public bool isGrounded;
    public Transform checkPos;
    public float checkRadius;
    public LayerMask groundMask;

    // find player variables
    public Transform player;
    float nextTimeToSearch;

    Rigidbody2D rb;
    Animator animator;
    EnemyKnockback kb;
    float localScale;

    [Header("OnCollisionStay DMG/Timer")]
    public float damageTimerCountdown;
    float damageTimer;
    bool doDamage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        kb = GetComponent<EnemyKnockback>();

        localScale = transform.localScale.x;  // store transform.localscale value
        damageTimer = damageTimerCountdown;
        jumpTimer = jumpTimerCountdown;
    }

    // Update is called once per frame
    void Update()
    {   
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // check if enemy is grounded
        isGrounded = Physics2D.OverlapCircle(checkPos.position, checkRadius, groundMask);

        if (facingRight == true)
        {
            // check if there is a wall right side
            stopMoving = Physics2D.Raycast(transform.position, Vector2.right, wallRaycastDistance, wallMask);

            // check if boss can jump
            if (animator.GetBool("Attack") == false)
            {
                if (jumpTimer <= 0)
                {
                    int randomInt = Random.Range(0, 2);
                    if (randomInt == 0)
                    {
                        // SHORT JUMP RAYCAST
                        RaycastCheckForJUMPING(Vector2.right, raycastDistanceForJump1, jumpSpeedsX[0], jumpSpeedsY[0]);
                    }
                    else
                    {
                        // LONG JUMP RAYCAST
                        RaycastCheckForJUMPING(Vector2.right, raycastDistanceForJump2, jumpSpeedsX[1], jumpSpeedsY[1]);
                    }

                    // reset timer 
                    jumpTimer = jumpTimerCountdown;
                }
                else
                    jumpTimer -= Time.deltaTime;
            }
        }
        else // LEFT SIDE
        {   
            // check if there is a wall left side
            stopMoving = Physics2D.Raycast(transform.position, Vector2.left, wallRaycastDistance, wallMask);
            animator.SetBool("stopMoving", stopMoving);

            // check if boss can jump
            if (animator.GetBool("Attack") == false)
            {
                if (jumpTimer <= 0)
                {
                    int randomInt = Random.Range(0, 2);
                    if (randomInt == 0)
                    {
                        // SHORT JUMP RAYCAST
                        RaycastCheckForJUMPING(Vector2.left, raycastDistanceForJump1, jumpSpeedsX[0], jumpSpeedsY[0]);
                    }
                    else
                    {
                        // LONG JUMP RAYCAST
                        RaycastCheckForJUMPING(Vector2.left, raycastDistanceForJump2, jumpSpeedsX[1], jumpSpeedsY[1]);
                    }

                    // reset timer 
                    jumpTimer = jumpTimerCountdown;
                }
                else
                    jumpTimer -= Time.deltaTime;
            }
        }

        // OnCollisionStay damage timer
        if (doDamage == true)
        {
            if (damageTimer <= 0)
            {   
                // dmg player
                Player _player = player.GetComponent<Player>();
                DamagePlayerCollision(_player);

                damageTimer = damageTimerCountdown;
            }
            else
                damageTimer -= Time.deltaTime;
        }
    }

    // -----------------------------------------------
    // Jump Functions
    void RaycastCheckForJUMPING(Vector2 direction, float rcDistance, float jumpSpeedX, float jumpSpeedY)
    {
        if (jumpTimer <= 0)
        {
            // check right raycast for jumping
            if (Physics2D.Raycast(transform.position, direction, rcDistance, wallMask) == false
                && isGrounded == true && animator.GetBool("isJumping") == false)
            {
                // 0 is NO jump and 1 is YES jump (50% chance to jump)
                int number = Random.Range(0, 2);
                if (number == 1)
                {
                    animator.SetBool("Run", false);
                    animator.SetBool("isJumping", true);
                    Jump(jumpSpeedX, jumpSpeedY);
                }
            }

            // reset timer
            jumpTimer = jumpTimerCountdown;
        }
        else
            jumpTimer -= Time.deltaTime;
    }

    void Jump(float jumpSpeedsX, float jumpSpeedsY)
    {
        rb.velocity = Vector2.zero;

        if (facingRight == true)
            rb.velocity = new Vector2(jumpSpeedsX, jumpSpeedsY);
        else
            rb.velocity = new Vector2(-jumpSpeedsX, jumpSpeedsY);
    }

    // used in Bandit_jumping.cs Update()
    public bool CheckForWall()
    {
        if (facingRight == true)
        {   
            // if raycast hits wall, return true
            if (Physics2D.Raycast(transform.position, Vector2.right, wallRaycastDistance, wallMask))
                return true;        
        }
        else
        {
            if (Physics2D.Raycast(transform.position, Vector2.left, wallRaycastDistance, wallMask))
                return true;
        }

        // no walls are detected
        return false;
    }

    // -----------------------------------------------

    public void CheckForPlayerRaycast(Vector2 direction, Color raycastColor)
    {   
        // if player is in range
        if (Physics2D.Raycast(transform.position, direction, raycastDistance, playerMask) == true)
        {   
            if (nextTimeAttack <= Time.time)
            {
                animator.SetBool("Attack", true);
                nextTimeAttack = Time.time + 1f / attackRate;

                Debug.DrawRay(transform.position, direction * raycastDistance, raycastColor, 5f);
            }
        }
    }

    public void FacePlayer()
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-localScale, localScale, localScale);
            facingRight = true;
        }
        else
        {
            transform.localScale = new Vector3(localScale, localScale, localScale);
            facingRight = false;
        }
    }

    void DamagePlayerCollision(Player player)
    {   
        // take dmg
        int halfDamage = damage / 2;
        player.DamagePlayer(halfDamage);
    }
    
    // -------------------------------------------------
    // TRIGGER FUNCTIONS

    // on trigger is so that when the player collides with boss when its jumping, player gets knocked back
    // but boss ignores physics collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            DamagePlayerCollision(player);

            // knock back player
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            PlayerKnockback.instance.KnockBackPlayer(playerCollider, gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doDamage = false;
            damageTimer = damageTimerCountdown;
        }
    }

    // ------------------------------------------------
    // COLLISION FUNCTIONS

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {       
            DamagePlayerCollision(player);

            // knock back player
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            PlayerKnockback.instance.KnockBackPlayer(playerCollider, gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            doDamage = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            doDamage = false;
            damageTimer = damageTimerCountdown;
        }
    }

    // -------------------------------------------------

    public void Knockback()
    {
        kb.KnockBackTimer -= Time.deltaTime;

        if (kb.knockFromRight)
            rb.velocity = new Vector2(-kb.knockBackAmount, 0f);
        else
            rb.velocity = new Vector2(kb.knockBackAmount, 0f);
    }

    // -----------------------------
    // animation frame functions

    // Attack function (called on anim)
    void Attack()
    {
        AudioManager.instance.Play("Bandit");

        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage);

            Collider2D playerCol = player.GetComponent<Collider2D>();
            PlayerKnockback.instance.KnockBackPlayer(playerCol, gameObject);
        }
    }

    // ------------------------------
    public void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextTimeToSearch = Time.time + 0.75f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        else if (checkPos == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.DrawWireSphere(checkPos.position, checkRadius);
    }
}
