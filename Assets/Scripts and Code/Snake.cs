using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{   
    public int damage;
    public float moveSpeed;
    public LayerMask groundMask;
    public Transform checkPos;

    public bool moveRight;

    [Header("Waypoint Movement")]
    public bool isWaypointMovement;
    public float leftCoordinate;
    public float rightCoordinate;

    // Update is called once per frame
    void Update()
    {
        if (isWaypointMovement == true)
        {
            if (transform.parent.position.x > rightCoordinate)
            {
                moveRight = false;
                transform.localScale = new Vector2(-1, 1);
            }
            else if (transform.parent.position.x < leftCoordinate)
            {
                moveRight = true;
                transform.localScale = new Vector2(1, 1);
            }
        }
        else
        {
            if (Physics2D.Raycast(checkPos.position, Vector2.down, 2f, groundMask) == false)
            {
                if (moveRight)
                {
                    moveRight = false;
                    transform.localScale = new Vector2(-1, 1);
                }
                else
                {
                    moveRight = true;
                    transform.localScale = new Vector2(1, 1);
                }
            }
        }

        if (moveRight)
            transform.parent.position = new Vector2(transform.parent.position.x + moveSpeed * Time.deltaTime, transform.parent.position.y);
        else
            transform.parent.position = new Vector2(transform.parent.position.x - moveSpeed * Time.deltaTime, transform.parent.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(collision, gameObject);
        }
    }
}
