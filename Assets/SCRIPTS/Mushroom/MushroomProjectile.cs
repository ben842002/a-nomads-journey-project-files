using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomProjectile : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;

    public GameObject effectPrefab;

    public int damage;
    public float moveSpeed;
    public float destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        Vector2 moveDirection = (player.position - transform.position).normalized * moveSpeed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        if (player == null)
            Effect(transform.position);           
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            Vector2 closestPos = collision.ClosestPoint(transform.position);
            Effect(closestPos);
        }
    }

    void Effect(Vector2 spawnPos)
    {
        GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(gameObject);
    }
}
