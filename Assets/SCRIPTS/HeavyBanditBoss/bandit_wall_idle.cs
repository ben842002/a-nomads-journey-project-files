using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bandit_wall_idle : StateMachineBehaviour
{
    HeavyBandit b;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        b = animator.GetComponent<HeavyBandit>();

        // weird bug happens when boss jumps right intro corner of wall and remains idle
        // looking at the anim parameters, attack is set to true (reason: not known yet)
        if (animator.GetBool("Attack") == true)
            animator.SetBool("Attack", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (b.player == null)
        {
            b.FindPlayer();
            return;
        }

        b.FacePlayer();

        // check if player is in range to attack
        if (b.facingRight == true)
            b.CheckForPlayerRaycast(Vector2.right, Color.red);
        else
            b.CheckForPlayerRaycast(Vector2.left, Color.green);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
