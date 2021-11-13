using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [SerializeField]
    public int damage = 9999;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            if (collision.GetComponent<Animator>().GetBool("isDead") == false)
            {
                // make sure player takes dmg no matter what
                collision.GetComponent<Player>().isInvulnerable = false;
                collision.GetComponent<Player>().DamagePlayer(damage);
            }

            PlayerKnockback.instance.KnockBackPlayer(collision, gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().DamageEnemy(damage);
        }
    }
}
