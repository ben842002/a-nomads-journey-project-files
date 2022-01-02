using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leechhag : MonoBehaviour
{
    public float attackRate;
    float nextTimeAttack = 0f;
    public float raycastDistance;
    public Vector3 offset;

    [Header("Attack")]
    public int damage;
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public float attackRadius;
    public LayerMask playerMask;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.Raycast(transform.position + offset, Vector2.right, raycastDistance, playerMask) == true &&
            nextTimeAttack <= Time.time)
        {
            animator.SetTrigger("Right");
            Debug.DrawRay(transform.position + offset, Vector2.right * raycastDistance, Color.red);

            nextTimeAttack = Time.time + 1f / attackRate;
        }
        else if (Physics2D.Raycast(transform.position + offset, Vector2.left, raycastDistance, playerMask) == true
            && nextTimeAttack <= Time.time)
        {
            animator.SetTrigger("Left");
            Debug.DrawRay(transform.position + offset, Vector2.left * raycastDistance, Color.white);

            nextTimeAttack = Time.time + 1f / attackRate;
        }
    }

    void DamagePlayer(Transform attackPointDirection)
    {
        AudioManager.instance.Play("Leechhag");

        Collider2D player = Physics2D.OverlapCircle(attackPointDirection.position, attackRadius, playerMask);
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(player, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(collision, gameObject);
        }
    }

    // functions below and called on anim frames
    void AttackLeft()
    {
        DamagePlayer(attackPointLeft);
    }

    void AttackRight()
    {
        DamagePlayer(attackPointRight);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPointLeft == null)
            return;

        if (attackPointRight == null)
            return;

        Gizmos.DrawWireSphere(attackPointLeft.position, attackRadius);
        Gizmos.DrawWireSphere(attackPointRight.position, attackRadius);
    }
}
