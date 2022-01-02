using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : StateMachineBehaviour
{
    PlayerMovement movement;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement = animator.GetComponent<PlayerMovement>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // play falling animation when player is not grounded but isnt jumping
        if (movement.isGrounded == false && animator.GetBool("isJumping") == false
            && animator.GetBool("isDashing") == false && movement.isDoubleJumping == false) 
        {
            animator.SetLayerWeight(1, 1);
            animator.Play("player_jump", 1, 0.5f);
        }
    }
}
