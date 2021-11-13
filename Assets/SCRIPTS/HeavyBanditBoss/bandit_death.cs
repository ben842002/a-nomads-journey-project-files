using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bandit_death : StateMachineBehaviour
{
    SpawnProjectiles sp;
    Enemy enemy;
    Rigidbody2D rb;
    HeavyBandit b;
    BoxCollider2D boxCol;
    BossFight bossF;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // reference variables
        sp = animator.GetComponent<SpawnProjectiles>();
        enemy = animator.GetComponent<Enemy>();
        rb = animator.GetComponent<Rigidbody2D>();
        b = animator.GetComponent<HeavyBandit>();
        boxCol = animator.GetComponent<BoxCollider2D>();
        bossF = FindObjectOfType<BossFight>();

        // remove boss from enemies list so walls can move back to orig position
        bossF.enemies.Remove(animator.gameObject);
        
        // destroy canvas
        GameObject canvasObj = animator.GetComponentInChildren<Canvas>().gameObject;
        Destroy(canvasObj);

        // disable immediate scripts
        Destroy(sp);
        Destroy(enemy);
        Destroy(b);

        // make boss fall downwards on Y axis only
        rb.velocity = new Vector2(0f, rb.velocity.y);

        // addresses the possibility that boss dies during jumping (trigger is turned on)
        boxCol.isTrigger = false;

        // change tag and layer so player cant attack again
        animator.gameObject.tag = "Untagged";
        animator.gameObject.layer = 10;
        Physics2D.IgnoreLayerCollision(10, 8);
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
