using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyegob : MonoBehaviour
{   
    Animator animator;
    EnemyKnockback kb;
    Rigidbody2D rb;

    public bool facingRight = true;
    public int damage;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float attackRate;
    float nextTimeAttack;

    [Header("Player")]
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask playerMask;
    Transform player;
    float nextSearch;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        kb = GetComponent<EnemyKnockback>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (kb.KnockBackTimer <= 0)
            FacePlayer();
    }

    // called in eyegob_projectile_shoot anim state script
    public void CheckIfPlayerInRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, checkRadius, playerMask);
        if (player != null)
        {
            if (nextTimeAttack <= Time.time)
            {
                animator.SetBool("Projectile", true);
                nextTimeAttack = Time.time + 1f / attackRate;
            }
        }
    }

    // called on animation frame for attacking
    void ShootProjectile()
    {
        AudioManager.instance.Play("EnemyShoot");
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(damage / 2);

            Collider2D playerCol = player.GetComponent<Collider2D>();
            PlayerKnockback.instance.KnockBackPlayer(playerCol, gameObject);
        }
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {   
            if (facingRight == false)
            {
                transform.localScale = new Vector2(1, 1);
                facingRight = true;
            }
        }
        else
        {   
            if (facingRight == true)
            {
                transform.localScale = new Vector2(-1, 1);
                facingRight = false;
            }
        }
    }

    void FindPlayer()
    {
        if (nextSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextSearch = Time.time + 0.75f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
