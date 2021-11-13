using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit_jumping : StateMachineBehaviour
{
    HeavyBandit b;
    //Rigidbody2D rb;
    Collider2D col;

    //// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        b = animator.GetComponent<HeavyBandit>();
        col = animator.GetComponent<Collider2D>();
        
        col.isTrigger = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // if boss is front of wall, set collider to nontrigger so they dont go thru it 
        if (b.CheckForWall() == true)
        {
            col.isTrigger = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (col.isTrigger == true)
            col.isTrigger = false;
    }
}
