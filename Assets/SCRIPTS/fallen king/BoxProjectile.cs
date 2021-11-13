using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxProjectile : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;

    public int damage;
    public float speed;
    public GameObject hitEffect;
    public string[] tags;

    // Start is called before the first frame update
    void Start()
    {   
        // destroy projectile if player is not found
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult != null)
            player = sResult.transform;
        else
            Destroy(gameObject);

        rb = GetComponent<Rigidbody2D>();

        Vector2 moveDir = (player.position - transform.position).normalized * speed;
        rb.velocity = new Vector2(moveDir.x, moveDir.y);

        float rotationRandom = Random.Range(0f, 360f);
        transform.Rotate(new Vector3(0, 0, rotationRandom));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            Effect(collision);

            Destroy(gameObject);

        }
        else if (tags.Contains(collision.tag))
        {
            Effect(collision);
            Destroy(gameObject);
        }
    }

    void Effect(Collider2D collision)
    {
        Vector2 spawnPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(hitEffect, spawnPos, Quaternion.identity);
        Destroy(effect, 1f);
    }
}
