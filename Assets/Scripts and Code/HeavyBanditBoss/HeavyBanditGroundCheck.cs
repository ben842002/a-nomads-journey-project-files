using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBanditGroundCheck : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("Run", true);
        }
    }
}
