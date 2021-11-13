using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileSpawn : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<HomingMissile>().enabled = true;
        animator.GetComponent<Collider2D>().enabled = true;
    }
}
