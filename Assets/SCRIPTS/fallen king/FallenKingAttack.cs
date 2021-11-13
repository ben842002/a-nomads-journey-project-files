using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenKingAttack : MonoBehaviour
{
    FallenKing fk;
    Animator animator;

    [SerializeField] int damage;

    [Header("Regular Attack")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float raycastDistance;
    [SerializeField] Vector3 offset;

    [Header("AttackRate: Lower Values = longer intervals between attacks!")]
    [SerializeField] float attackRate;
    float nextAttack;

    // Start is called before the first frame update
    void Start()
    {
        fk = GetComponent<FallenKing>();
        animator = GetComponent<Animator>();
    }
    
    // ----------------------------------------------
    // called in fk_run

    public bool PlayerInRangeForAttack(Vector2 dir)
    {
        bool TorF = Physics2D.Raycast(transform.position + offset, dir, raycastDistance, playerMask);
        return TorF;
    }

    public void RandomizeAttack()
    {
        if (nextAttack <= Time.time)
        {
            int random = Random.Range(0, 3);
            if (random == 0)
                animator.SetTrigger("ChargedAttack");
            else
                animator.SetTrigger("RegularAttack");

            nextAttack = Time.time + 1f / attackRate;
        }
    }

    // ----------------------------------------------
    // functions called on attack anim frames

    void RegularAttack()
    {
        AudioManager.instance.Play("FKAttack");
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (player != null)
            DamagePlayer(player, damage);
    }

    void ChargedAttack()
    {
        AudioManager.instance.Play("FKAttackE");
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (player != null)
            DamagePlayer(player, damage * 2);
    }

    // ----------------------------------------------

    void DamagePlayer(Collider2D player, int damage)
    {
        player.GetComponent<Player>().DamagePlayer(damage);
        PlayerKnockback.instance.KnockBackPlayer(player, gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawRay(transform.position + offset, Vector2.left * raycastDistance);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
