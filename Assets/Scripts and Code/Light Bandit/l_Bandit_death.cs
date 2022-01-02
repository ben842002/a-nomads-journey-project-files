using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class l_Bandit_death : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.GetComponent<Enemy>());
        Destroy(animator.GetComponent<EnemyKnockback>());
        Destroy(animator.GetComponent<L_Bandit>());

        GameObject canvasObject = animator.transform.parent.GetComponentInChildren<Canvas>().gameObject;
        if (canvasObject != null)
            Destroy(canvasObject);

        animator.gameObject.tag = "Untagged";
        animator.gameObject.layer = 10;
        Physics2D.IgnoreLayerCollision(10, 8);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
