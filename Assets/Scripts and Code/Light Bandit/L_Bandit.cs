using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Bandit : MonoBehaviour
{   
    public bool facingRight;
    float fallBoundary;

    [Header("Grounded")]
    public bool isGrounded;
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;

    [Header("Movement")]
    public float speed;

    [Header("Player Range Detection")]
    public float minimumRange;

    [Header("Raycast for Attacking")]
    [SerializeField] float raycastDistance;

    [Header("Attack")]
    [SerializeField] Transform attackPoint;
    [SerializeField] int damage;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float attackRate;
    float nextTimeAttack;

    Animator animator;
    Rigidbody2D rb;
    EnemyKnockback kb;

    // find player;
    [HideInInspector] public Transform player;
    float nextSearch;

    // local scale
    float ls;

    // oncollisionStay dmg
    float timerCountdown = .5f;
    float timer;
    bool doDmgTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();

        ls = transform.localScale.x;
        timer = timerCountdown;

        fallBoundary = GameMaster.gm.fallBoundary;
    }

    private void Update()
    {
        if (player == null)
        {
            doDmgTimer = false;
            FindPlayer();
            return;
        }

        // oncollisionstay dmg
        if (doDmgTimer == true)
        {
            if (timer <= 0)
            {
                // damage
                int halfDamage = damage / 2;
                if (player != null)
                    player.GetComponent<Player>().DamagePlayer(halfDamage);

                // knockback
                Collider2D playerCol = player.GetComponent<Collider2D>();
                PlayerKnockback.instance.KnockBackPlayer(playerCol, gameObject);
                timer = timerCountdown;
            }
            else
                timer -= Time.deltaTime;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, groundMask);
        if (isGrounded == true)
        {
            if (animator.GetBool("isDead") == true)
            {
                animator.SetBool("Run", false);
                animator.SetBool("Attack", false);

                if (animator.GetBool("Attack") == false && animator.GetBool("Run") == false)
                {
                    Destroy(animator.GetComponentInParent<Rigidbody2D>());
                    Destroy(GetComponent<Collider2D>());
                    Destroy(this);
                }
            }
        }

        // kill enemy at a certain y distance
        if (transform.parent.position.y <= fallBoundary)
            Destroy(transform.parent.gameObject);

        // if player is in range, trigger run animation state, and if not, just play idle state
        if (Vector2.Distance(transform.parent.position, player.position) <= minimumRange)
        {
            if (animator.GetBool("Run") == false)
                animator.SetBool("Run", true);
        }
        else
            animator.SetBool("Run", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {   
            // damage
            int halfDamage = damage / 2;
            player.DamagePlayer(halfDamage);

            // knockback
            Collider2D playerCol = player.GetComponent<Collider2D>();
            PlayerKnockback.instance.KnockBackPlayer(playerCol, gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            doDmgTimer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            doDmgTimer = false;
            timer = timerCountdown;
        }
    }

    // called on attack anim frame
    void Attack()
    {
        AudioManager.instance.Play("Bandit");

        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(player, gameObject);
        }
    }

    // ------------------------------------------------
    // These functions below are called in the run animation state script

    public void CheckForPlayerRaycast(Vector2 direction, Color raycastColor)
    {
        if (Physics2D.Raycast(transform.position, direction, raycastDistance, playerMask) == true && nextTimeAttack <= Time.time)
        {
            animator.SetBool("Attack", true);
            Debug.DrawRay(transform.position, direction * raycastDistance, raycastColor, 5f);

            nextTimeAttack = Time.time + 1f / attackRate;
        }
    }

    public void Knockback()
    {
        kb.KnockBackTimer -= Time.deltaTime;

        if (facingRight == true)
            rb.velocity = new Vector2(-kb.knockBackAmount, 0f);
        else
            rb.velocity = new Vector2(kb.knockBackAmount, 0f);
    }

    public void FaceObject(Vector2 _object)
    {
        if (_object.x > transform.position.x)
        {
            transform.localScale = new Vector2(-ls, ls);
            facingRight = true;
        }
        else
        {
            transform.localScale = new Vector2(ls, ls);
            facingRight = false;
        }
    }

    public void FindPlayer()
    {
        if (nextSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextSearch = Time.time + 0.5f;
        }
    }

    // --------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
