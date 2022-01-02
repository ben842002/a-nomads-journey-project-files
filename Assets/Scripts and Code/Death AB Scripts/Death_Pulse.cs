using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Death_Pulse : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage = 20;

    [SerializeField]
    GameObject hitEffectPrefab;

    [Header("NonPlayer Collider Tags")]
    [SerializeField] string[] tags;

    Rigidbody2D rb;
    Death d;

    float ls;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        d = FindObjectOfType<Death>();

        ls = transform.localScale.x;

        if (d.facingRight == true)
            rb.velocity = new Vector2(speed, 0f);
        else
        {
            transform.localScale = new Vector2(-ls, ls);
            rb.velocity = new Vector2(-speed, 0f);
        }
    }

    /// <summary>
    /// In the boss fight, there is a scenario where both if and the else if are called. This is
    /// because the projectile is spawned on top of wall and player when near the edge of the fighting zone.
    /// To fix this: just increase the wall check raycast distance to avoid the problem completely
    /// (Writing this to just give more info when you look back at this boss fight code)
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(collision, gameObject);

            HitEffect(collision);
        }
        else if (tags.Contains(collision.tag))
        {
            HitEffect(collision);
        }
    }

    void HitEffect(Collider2D collision)
    {   
        Vector2 hitPos = collision.ClosestPoint(transform.position);
        GameObject hitEffect = Instantiate(hitEffectPrefab, hitPos, Quaternion.identity);
        Destroy(hitEffect, 0.5f);

        Destroy(gameObject);
    }
}
