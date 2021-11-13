using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    Animator anim;

    public bool isGrounded;
    [SerializeField] float moveSpeed;

    [Header("isGrounded")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckDist;
    public bool facingRight;

    [Header("Attack")]
    [SerializeField] int damage;
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius;
    [SerializeField] float raycastDistance;
    [SerializeField] float attackRate;
    float nextTimeAttack;

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
            doDmg = false;
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

        // check if grounded (used in skeleton_walk)
        isGrounded = Physics2D.Raycast(groundCheckPos.position, Vector2.down, groundCheckDist, playerMask);

        // run towards player if in range
        playerInRange = Physics2D.OverlapCircle(transform.position, checkRadius, playerMask);
        if (playerInRange == true)
            anim.SetBool("run", true);
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

    // called in skeleton_walk
    public void DetectPlayerForAttack(Vector2 dir, Color color)
    {
        if (Physics2D.Raycast(transform.parent.position, dir, raycastDistance, playerMask) == true)
        {
            Debug.DrawRay(transform.parent.position, dir * raycastDistance, color, 1f);

            // random chance for shield. Everything else is attack
            int num = Random.Range(0, 3);
            if (num == 0)
                anim.SetTrigger("shield");
            else
            {
                if (nextTimeAttack <= Time.time)
                {
                    anim.SetTrigger("Attack");
                    nextTimeAttack = Time.time + 1f / attackRate;
                }
            }
        }
    }

    // called in skeleton_walk
    public void RunTowardsTarget(Vector3 target)
    {
        transform.parent.position = Vector2.MoveTowards(transform.parent.position,
            new Vector2(target.x, transform.parent.position.y), moveSpeed * Time.deltaTime);
    }

    // called on attack anim frame
    public void Attack()
    {
        AudioManager.instance.Play("Skeleton");
        Collider2D player = Physics2D.OverlapCircle(attackPos.position, attackRadius, playerMask);
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(player, transform.parent.gameObject);
        }
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
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);

        Gizmos.DrawRay(groundCheckPos.position, Vector2.down * groundCheckDist);
    }
}
