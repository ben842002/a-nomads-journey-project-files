using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblin_walk : StateMachineBehaviour
{
    Goblin g;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        g = animator.GetComponent<Goblin>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (g.player == null)
            return;

        if (g.playerInRange == true)
        {
            // face player and run towards them
            g.FaceGameObject(g.player.position);
            g.RunTowardsTarget(g.player.position);

            // check for possible attack
            if (g.facingRight)
                g.DetectPlayerForAttack(Vector2.right, Color.red);
            else
                g.DetectPlayerForAttack(Vector2.left, Color.blue);
        }
        else
        {
            if (animator.transform.parent.position.x == g.originalPos.x)
                animator.SetBool("run", false);
            else
            {
                g.FaceGameObject(g.originalPos);
                g.RunTowardsTarget(g.originalPos);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
