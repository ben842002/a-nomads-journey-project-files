using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death_attacklayer0 : StateMachineBehaviour
{
    Death d;
    EnemyKnockback kb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        d = animator.GetComponent<Death>();
        kb = animator.GetComponent<EnemyKnockback>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // dont knock back during enemy attack
        if (kb.KnockBackTimer > 0)
            kb.KnockBackTimer = -1;

        // addresses bug where boss gets flinged back when attacking (maybe knockback rb code gets run once?)
        animator.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 0);
        animator.SetBool("Run", true);
        animator.SetBool("Projectile", false);
        animator.SetBool("Summon", false);
    }
}
