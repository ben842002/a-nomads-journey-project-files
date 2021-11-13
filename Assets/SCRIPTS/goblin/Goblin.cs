using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    Animator anim;

    public bool facingRight;
    [SerializeField] float moveSpeed;

    [Header("Attack")]
    [SerializeField] int damage;
    [SerializeField] PolygonCollider2D attackCol;
    [SerializeField] float raycastDistance;

    [Header("Bomb")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform bombSpawnPos;

    [Header("Player In Range")]
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask playerMask;
    public bool playerInRange;

    [HideInInspector] public Transform player;
    float nextTimeSearch;

    [HideInInspector] public Vector3 originalPos;

    [Header("OnCol Damage")]
    public float colTimerCountdown;
    float timer;
    bool doDmg;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        originalPos = transform.position;
        timer = colTimerCountdown;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            doDmg = false; // possible bug where bool is still true
            return;
        }

        if (doDmg == true)
        {
            if (timer <= 0)
            {
                player.GetComponent<Player>().DamagePlayer(damage / 2);
                PlayerKnockback.instance.KnockBackPlayer(player.GetComponent<Collider2D>(), transform.parent.gameObject);
                timer = colTimerCountdown;
            }
            else
                timer -= Time.deltaTime;
        }

        playerInRange = Physics2D.OverlapCircle(transform.parent.position, checkRadius, playerMask);
        if (playerInRange == true)
        {
            if (anim.GetBool("run") == false)
            {
                anim.SetBool("run", true);
                GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    public void FaceGameObject(Vector2 objectPosition)
    {
        if (objectPosition.x > transform.parent.position.x)
        {
            facingRight = true;
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            facingRight = false;
            transform.localScale = new Vector2(-1, 1);
        }
    }

    // called in goblin_walk
    public void DetectPlayerForAttack(Vector2 dir, Color color)
    {
        if (Physics2D.Raycast(transform.parent.position, dir, raycastDistance, playerMask) == true)
        {
            Debug.DrawRay(transform.parent.position, dir * raycastDistance, color, 1f);

            // random chance for bomb. Everything else is attack
            int num = Random.Range(0, 3);
            if (num == 0)
                anim.SetTrigger("Bomb");
            else
                anim.SetTrigger("Attack");          
        }
    }

    // called in goblin_walk
    public void RunTowardsTarget(Vector3 target)
    {
        transform.parent.position = Vector2.MoveTowards(transform.parent.position,
            new Vector2(target.x, transform.parent.position.y), moveSpeed * Time.deltaTime);
    }

    // called on attack anim frame
    public void Attack()
    {
        AudioManager.instance.Play("GoblinAttack");
        bool overlap = attackCol.IsTouching(player.GetComponent<Collider2D>());
        if (overlap)
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(player.GetComponent<Collider2D>(), transform.parent.gameObject);
        }
    }

    // called on bomb anim frame
    public void Bomb()
    {
        AudioManager.instance.Play("GoblinThrow");

        GameObject bomb = Instantiate(bombPrefab, bombSpawnPos.position, Quaternion.identity);
        Bomb b = bomb.GetComponent<Bomb>();

        // randomize velocity (small interval)
        b.Xspeed = Random.Range(b.Xspeed - 1, b.Xspeed + 1);
        b.Yspeed = Random.Range(b.Yspeed - 1, b.Xspeed + 1);

        // make bomb move left if goblin is facing left
        if (facingRight == false)
            b.Xspeed *= -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(damage / 2);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, transform.parent.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            doDmg = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            doDmg = false;
            timer = colTimerCountdown;
        }
    }

    void FindPlayer()
    {
        if (nextTimeSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextTimeSearch = Time.time + 0.75f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.parent.position, checkRadius);
    }
}
