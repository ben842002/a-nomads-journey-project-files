using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    public int damage = 35;
    public float speed = 5f;
    public GameObject collisionParticleEffect;
    Transform target;

    public float rotateSpeed = 200f;

    public string[] Tags = { "MovingPlatform", "Ground" };

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // destroy player
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        // get cross produce to help rotate sprite correctly
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            CinemachineShake.instance.ShakeCamera(10f, 0.1f);

            CreateEffect(collision);
            Destroy(gameObject);
        }

        if (Tags.Contains(collision.tag))
        {
            CreateEffect(collision);
            Destroy(gameObject);
        }
    }

    void CreateEffect(Collider2D collision)
    {
        Vector3 hitPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(collisionParticleEffect, hitPos, transform.rotation);
        Destroy(effect, 1f);
    }
}
