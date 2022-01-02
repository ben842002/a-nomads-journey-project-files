using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Vertical_Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public int damage;
    public float speed;
    public float destroyTime;

    [Header("Hit Effect")]
    public GameObject hitEffect;
    public float effectDestroyTime;

    /* The SpriteRenderer is turned off because there is a single frame that displays 
     * the full sprite before the animator plays.
     */

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0f, -speed);
        Destroy(gameObject, destroyTime);
    }

    public void SetSpeed()
    {
        rb.velocity = new Vector2(0f, -speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            Effect(collision);
        }
    }

    void Effect(Collider2D collision)
    {
        Vector2 pos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(hitEffect, pos, Quaternion.identity);

        Destroy(effect, effectDestroyTime);
    }
}
