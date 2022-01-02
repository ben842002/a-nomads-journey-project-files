using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderDamage : MonoBehaviour
{
    PlayerCombat combat;

    private void Start()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }

    // some enemies will have a trigger collider while others will be collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((combat.enemyMask.value & 1 << collision.gameObject.layer) != 0)
        {
            combat.DamageEnemy(collision, combat.attackDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((combat.enemyMask.value & 1 << collision.gameObject.layer) != 0)
        {
            combat.DamageEnemy(collision.collider, combat.attackDamage);
        }
    }
}
