using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class l_Bandit_run : StateMachineBehaviour
{
    L_Bandit b;
    EnemyKnockback kb;
    Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        b = animator.GetComponent<L_Bandit>();
        kb = animator.GetComponent<EnemyKnockback>();
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (kb.KnockBackTimer <= 0)
        {
            if (b.player != null)
            {   
                // face the player
                b.FaceObject(b.player.position);

                // check if player is in range for attack
                if (b.facingRight == true)
                    b.CheckForPlayerRaycast(Vector2.right, Color.red);
                else
                    b.CheckForPlayerRaycast(Vector2.left, Color.green);

                if (b.isGrounded == true)
                {
                    // reset velocity
                    rb.velocity = Vector2.zero;

                    // move towards player
                    animator.transform.parent.position = Vector2.MoveTowards(animator.transform.parent.position,
                        new Vector2(b.player.position.x, animator.transform.parent.position.y), b.speed * Time.deltaTime);
                }
            }        
        }
        else
        {   
            // bug when player kills bandit (MissingReferenceException)
            if (rb != null)
            {
                // knock back the enemy
                b.Knockback();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
