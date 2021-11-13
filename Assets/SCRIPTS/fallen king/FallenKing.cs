using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenKing : MonoBehaviour
{
    public bool facingRight;
    [SerializeField] float moveSpeed;

    [Header("Collision")]
    [SerializeField] bool doDmg;
    [SerializeField] int collisionDamage;
    [SerializeField] float colTimerC;
    float colTimer;

    [Header("If the object is facing right, toggle isFlipped to true")]
    public bool isFlipped;  

    [HideInInspector] public Transform player;
    float nextSearch;

    private void Start()
    {
        colTimer = colTimerC;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            doDmg = false;
            FindPlayer();
            return;
        }

        // collision stay dmg
        if (doDmg == true)
        {
            if (colTimer <= 0)
            {
                player.GetComponent<Player>().DamagePlayer(collisionDamage);
                colTimer = colTimerC;
            }
            else
                colTimer -= Time.deltaTime;
        }
    }

    // -------------------------------------
    // Functions are called in fk_run

    /// <summary>
    /// this alternative to new Vector3(-1 , 1, 1) is better because there is no stuttering.
    /// </summary>
    public void FacePlayer()
    {   
        // when the player is to the LEFT and isFlipped is true, ROTATE 180 DEGREES
        if (player.position.x < transform.position.x && isFlipped == true)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
            facingRight = false;
        }
        // when the player is to the RIGHT and isFlipped is false, ROTATE 180 DEGREES
        else if (player.position.x > transform.position.x && isFlipped == false)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
            facingRight = true;
        }
    }

    public void MoveToPlayer()
    {
        Vector2 playerPositionX = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, playerPositionX, moveSpeed * Time.deltaTime);
    }

    // ------------------------------------
    // collision functions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(collisionDamage);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, gameObject);
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
            colTimer = colTimerC;
        }
    }

    // ------------------------------------

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
}
