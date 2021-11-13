using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit_run : StateMachineBehaviour
{
    HeavyBandit b;
    Rigidbody2D rb;
    EnemyKnockback kb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        b = animator.GetComponent<HeavyBandit>();
        rb = animator.GetComponent<Rigidbody2D>();
        kb = animator.GetComponent<EnemyKnockback>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (b.player == null)
        {
            b.FindPlayer();
            return;
        }

        if (kb.KnockBackTimer <= 0)
        {
            rb.velocity = Vector2.zero;
            b.FacePlayer();

            // check if player is in range to attack
            if (b.facingRight == true)           
                b.CheckForPlayerRaycast(Vector2.right, Color.red);        
            else           
                b.CheckForPlayerRaycast(Vector2.left, Color.green);
            
            // only move to the point where enemy is in range to attack (not directly to the same position as player)
            if (b.stopMoving == false)
            {
                // only move in the x axis
                animator.transform.position = Vector2.MoveTowards(animator.transform.position,
                    new Vector2(b.player.position.x, animator.transform.position.y), b.runSpeed * Time.deltaTime);
            }
        }
        else
            b.Knockback();
    }
}
