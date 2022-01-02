using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblin_death : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.GetComponent<Goblin>());
        Destroy(animator.GetComponent<Enemy>());

        //canvas object
        GameObject canvasObj = animator.transform.parent.GetComponentInChildren<Canvas>().gameObject;
        Destroy(canvasObj);

        animator.gameObject.tag = "Untagged";
        animator.gameObject.layer = 10;

        Physics2D.IgnoreLayerCollision(10, 8);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
