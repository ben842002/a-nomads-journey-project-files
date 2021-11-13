using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Horizontal_Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public int damage;
    public float speed;
    public float destroyTime;

    [Header("Hit Effect")]
    public GameObject hitEffect;
    public float effectDestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0f);

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            HitEffect(collision);
        }
    }

    void HitEffect(Collider2D collision)
    {
        Vector2 hitPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(hitEffect, hitPos, Quaternion.identity);

        Destroy(effect, effectDestroyTime);
    }
}
