using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death_jumping : StateMachineBehaviour
{
    Rigidbody2D rb;
    EnemyKnockback kb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        kb = animator.GetComponent<EnemyKnockback>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // dont knock back during enemy attack
        if (kb.KnockBackTimer > 0)
            kb.KnockBackTimer = -1;

        if (rb.velocity.y <= 0f)
            animator.SetBool("Fall", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

}
