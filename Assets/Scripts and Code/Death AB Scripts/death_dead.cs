using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death_dead : StateMachineBehaviour
{
    BossFight b;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        b = FindObjectOfType<BossFight>();
        b.enemies.Remove(animator.gameObject);

        animator.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // destroy canvas
        GameObject canvasObject = animator.GetComponentInChildren<Canvas>().gameObject;
        Destroy(canvasObject);

        // destroy components
        Destroy(animator.GetComponent<EnemyKnockback>());
        Destroy(animator.GetComponent<Death>());
        Destroy(animator.GetComponent<Death_Summon>());
        Destroy(animator.GetComponent<Enemy>());

        // change layer and tag so player cant attack or collide with object
        animator.gameObject.layer = 10;
        animator.gameObject.tag = "Untagged";
        Physics2D.IgnoreLayerCollision(8, 10);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
