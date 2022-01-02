using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack1 : StateMachineBehaviour
{
    PlayerCombat combat;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combat = animator.GetComponent<PlayerCombat>();
        combat.isAttacking = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combat.isAttacking = false;
    }
}
