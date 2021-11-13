using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpikes : MonoBehaviour
{
    Rigidbody2D rb;

    public int damage = 20;
    public float shakeDelay;
    public ParticleSystem particles;

    [Header("Hit Particles")]
    public GameObject particleHit;
    public float destroyTime = .75f;

    [Header("Player Check")]
    public float raycastDistance;
    public LayerMask playerMask;

    [SerializeField] bool falling;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, playerMask) == true)
        {   
            if (falling == false)
            {
                StartCoroutine(DropSpike());
                falling = true;

                Debug.DrawRay(transform.parent.position, Vector2.down * raycastDistance, Color.red, 5f);
            }
        }
    }

    IEnumerator DropSpike()
    {   
        // let particle system play before dropping
        particles.Play();

        yield return new WaitForSeconds(shakeDelay);
        rb.isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            Effect();

            // destroy parent game object
            Destroy(transform.parent.gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Effect();
            Destroy(transform.parent.gameObject);           
        }
    }

    void Effect()
    {
        GameObject effect = Instantiate(particleHit, transform.position, transform.rotation);
        Destroy(effect, destroyTime);
    }
}
