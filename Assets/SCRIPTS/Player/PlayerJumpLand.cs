using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpLand : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    LayerMask groundMask;

    readonly string jump = "isJumping";
    readonly string land = "Land";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        // check if player has landed on layers chosen
        if (GetComponent<Collider2D>().IsTouchingLayers(groundMask) == true && animator.GetBool(jump) == true)
        {
            AudioManager.instance.Play("PlayerLand");

            // trigger player land animation
            animator.SetBool(jump, false);
            animator.SetBool(land, true);
        }
    }
}
