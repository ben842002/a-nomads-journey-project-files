using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreadghoul : MonoBehaviour
{   
    public int collisionDamage;

    [Header("Attack")]
    public int damage = 20;
    public float attackSpeed;
    float nextTimeAttack;
    public LayerMask playerMask;
    public float playerRaycastDistance = 5f;
    public Transform attackPoint;
    public float attackRadius;

    [Header("Movement")]
    public LayerMask groundMask;
    public Transform groundDetect;
    public float raycastDistance;
    public float speed;

    bool moveRight = true;
    bool groundDetected = false;
    bool playerInRange = false;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {   
        // check for ground by raycast
        groundDetected = Physics2D.Raycast(groundDetect.position, Vector2.down, raycastDistance, groundMask);

        // check for player by raycast
        if (moveRight == true)
            CheckForPlayer(Vector2.right, playerRaycastDistance, Color.green);
        else
            CheckForPlayer(Vector2.left, playerRaycastDistance, Color.white);
    }

    // Update is called once per frame
    void Update()
    {   
        // check if player is in range to attack

        // move right or left ONLY if they are not attacking and not getting hit
        if (animator.GetBool("isAttacking") == false && animator.GetBool("Hurt") == false)
        {
            if (moveRight == true)
            {
                transform.parent.position = new Vector2(transform.parent.position.x + speed * Time.deltaTime, transform.parent.position.y);
                TriggerAttack();
            }
            else
            {
                transform.parent.position = new Vector2(transform.parent.position.x - speed * Time.deltaTime, transform.parent.position.y);
                TriggerAttack();
            }
        }

        // if there is no ground beneath  
        if (groundDetected == false)
        {   
            // turn left
            if (moveRight == true)
            {
                moveRight = false;
                transform.localScale = new Vector2(-1, 1);
            }
            else // turn right
            {
                moveRight = true;
                transform.localScale = new Vector2(1, 1);
            }
        }
    }

    // called in fixedUpdate
    void CheckForPlayer(Vector2 raycastDirection, float raycastDistance, Color debugRaycastColor)
    {
        if (Physics2D.Raycast(transform.parent.position, raycastDirection, raycastDistance, playerMask) == true)
        {
            playerInRange = true;
            Debug.DrawRay(transform.parent.position, raycastDirection * raycastDistance, debugRaycastColor, 5f);
        }
        else
            playerInRange = false;         
    }

    void TriggerAttack()
    {
        if (playerInRange == true && nextTimeAttack <= Time.time)
        {
            animator.SetBool("isAttacking", true);
            nextTimeAttack = Time.time + 1f / attackSpeed;
        }
    }

    // called on animation frame
    void Attack()
    {
        AudioManager.instance.Play("Dreadghoul");

        Collider2D playerCollider = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (playerCollider != null)
        {
            playerCollider.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(playerCollider, gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().DamagePlayer(collisionDamage);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider,gameObject);
        }
    }

    // called on anim frame
    void NotHurt()
    {
        animator.SetBool("Hurt", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
