using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Rigidbody2D rb;

    public int damage;
    public float Xspeed;
    public float Yspeed;
    public GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // going left or right code is in Goblin.cs bomb function
        rb.velocity = new Vector2(Xspeed, Yspeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);

            Effect();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Effect();
            Destroy(gameObject);
        }
    }

    void Effect()
    {
        AudioManager.instance.Play("BombExplosion");
        GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(_effect, 0.5f);
    }
}
