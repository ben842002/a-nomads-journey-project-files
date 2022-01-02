using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cross_vertical_speed : StateMachineBehaviour
{
    Death_Vertical_Projectile d;
    Collider2D col;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        d = animator.GetComponent<Death_Vertical_Projectile>();
        col = animator.GetComponent<Collider2D>();
        animator.GetComponent<SpriteRenderer>().enabled = true;

        d.enabled = false;
        col.enabled = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        d.enabled = true;
        col.enabled = true;
    }
}
