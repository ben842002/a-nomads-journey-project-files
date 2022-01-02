using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death_run : StateMachineBehaviour
{
    Death d;
    EnemyKnockback kb;
    Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        d = animator.GetComponent<Death>();
        kb = animator.GetComponent<EnemyKnockback>();
        rb = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (d.player == null)
        {
            d.FindPlayer();
            return;
        }

        if (kb.KnockBackTimer <= 0)
        {
            rb.velocity = Vector2.zero;

            // look toward the player
            d.FacePlayer();

            // possible jump if conditions are met
            d.Jump();

            // fire raycasts
            if (d.facingRight == true)
            {
                d.CheckForPlayerForAttack(Vector2.right, Color.red);
                d.CheckForWalls(Vector2.right, Color.red);
            }
            else
            {
                d.CheckForPlayerForAttack(Vector2.left, Color.green);
                d.CheckForWalls(Vector2.left, Color.white);
            }

            // move only when boss is not in range of a wall
            if (d.wallLocated == false)
            {
                animator.transform.position = Vector2.MoveTowards(animator.transform.position,
                    new Vector2(d.player.position.x, animator.transform.position.y), d.runSpeed * Time.deltaTime);
            }
        }
        else
        {
            kb.KnockBackTimer -= Time.deltaTime;

            if (kb.knockFromRight == true)
                rb.velocity = new Vector2(-kb.knockBackAmount, 0f);
            else
                rb.velocity = new Vector2(kb.knockBackAmount, 0f);
        }
    }
}
