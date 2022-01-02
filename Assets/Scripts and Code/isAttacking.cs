using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isAttacking : StateMachineBehaviour
{
    PlayerCombat combat;
    Rigidbody2D rb;
    PlayerMovement movement;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combat = animator.GetComponent<PlayerCombat>();
        movement = animator.GetComponent<PlayerMovement>();
        rb = animator.GetComponent<Rigidbody2D>();

        combat.isAttacking = true;

        // decrease values
        movement.moveSpeed -= movement.attackMovementDecrease;
        rb.velocity = Vector2.zero;
        rb.gravityScale -= movement.gravityDecrease;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combat.isAttacking = false;

        // avoid error in inspector where rb or movement cant be found because player died
        if (rb != null && movement != null)
        {
            // return to original values
            movement.moveSpeed += movement.attackMovementDecrease;
            rb.gravityScale += movement.gravityDecrease;
        }
    }
}
