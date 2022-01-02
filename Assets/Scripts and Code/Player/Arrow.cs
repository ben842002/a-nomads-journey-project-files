using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arrow : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerMovement player_m;
    public GameObject hitEffect;

    public float moveSpeed;
    public int damage;

    public float destroyTime;

    public string[] tags =
    {
        "Ground", "MovingPlatform"
    };

    float ls;

    // Start is called before the first frame update
    void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        player_m = FindObjectOfType<PlayerMovement>();
        ls = transform.localScale.x;

        if (player_m.facingRight)
            rb.velocity = new Vector2(moveSpeed, 0f);
        else
        {
            rb.velocity = new Vector2(-moveSpeed, 0f);
            transform.localScale = new Vector2(-ls, ls);
        }

        // in case arrow doesnt hit anything
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().DamageEnemy(damage);
            HitEffect(collision);
        }
        else if (tags.Contains(collision.tag))
        {
            HitEffect(collision);
        }
    }


    void HitEffect(Collider2D collision)
    {
        AudioManager.instance.Play("ArrowImpact");

        // hit effect on closest point
        Vector2 hitPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(hitEffect, hitPos, Quaternion.identity);
        Destroy(effect, 0.5f);

        // destroy the arrow
        Destroy(gameObject);
    }
}
