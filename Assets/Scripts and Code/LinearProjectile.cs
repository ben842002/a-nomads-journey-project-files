using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinearProjectile : MonoBehaviour
{
    public float movementSpeed;
    public int damage = 25;
    public float timeBeforeDestroy = 7f;

    public GameObject hitParticle;

    Rigidbody2D rb;
    Transform player;
    Vector2 moveDirection;

    [SerializeField] readonly string[] Tags = { "Ground", "Wall" };

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform; 
       
        // Move towards player
        if (player != null)
            Shoot();

        // destroy object in given seconds if it doesnt hit anything
        Destroy(gameObject, timeBeforeDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        // if player dies
        if (player == null)
        {
            PlayParticles();
            Destroy(gameObject);
        }
    }

    void Shoot()
    {
        /* Calculate direction of projectile. We do this by getting a direction and a magnitude.
         * When doing (player.transform.position - transform.position), we get BOTH direction and magnitude (length of the vector). However,
         * we only want the DIRECTION because we already have the magnitude which is movementSpeed. So in order
         * to "get rid of" the magnitude of the (player.transform.position - transform.position) vector, we normalize it. 
         * (Technically it's a vector with a magnitude of 1, but because 1 is the multiplicative identity, 
         * multiplying a magnitude (length which in this case is movementSpeed) by a normalized vector sends you a distance of that 
         * length in the direction of the normalized vector.)
         * FOR MORE INFO: https://prnt.sc/132pcex */
        moveDirection = (player.transform.position - transform.position).normalized * movementSpeed;

        // move gameObject at a constant velocity 
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
    }

    void PlayParticles()
    {
        // spawn particle on hit
        GameObject effect = Instantiate(hitParticle, transform.position, Quaternion.identity);
        Destroy(effect, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayParticles();

            Destroy(gameObject);
        }

        // use System.Linq to check for multiple tags (reduces redudancy of CompareTag()
        if (Tags.Contains(collider.tag))
        {
            PlayParticles();
            Destroy(gameObject);
        }
    }
}
