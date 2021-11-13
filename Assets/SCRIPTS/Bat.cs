using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, gameObject);
        }
    }
}
