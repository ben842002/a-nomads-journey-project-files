using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable_Homing_Script : StateMachineBehaviour
{
    HomingMissile homing;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        homing = animator.GetComponent<HomingMissile>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        homing.enabled = true;
    }
}
