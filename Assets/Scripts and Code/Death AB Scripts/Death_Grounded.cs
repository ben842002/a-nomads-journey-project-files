using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Grounded : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("Fall", false);
            rb.velocity = Vector2.zero;
        }
    }
}
