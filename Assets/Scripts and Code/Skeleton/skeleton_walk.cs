using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeleton_walk : StateMachineBehaviour
{
    Skeleton s;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        s = animator.GetComponent<Skeleton>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator a, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (s.player == null)
            return;

        if (s.isGrounded == true)
            a.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;

        if (s.playerInRange == true)
        {   
            // run towards player and face them
            s.RunTowardsTarget(s.player.position);
            s.FaceGameObject(s.player.position);

            // raycast for possible attack
            if (s.facingRight == true)
            {
                s.DetectPlayerForAttack(Vector2.right, Color.blue);
            }
            else
            {
                s.DetectPlayerForAttack(Vector2.left, Color.white);
            }
        }
        else
        {
            // face correct direction
            s.FaceGameObject(s.originalPos);

            // stay idle when in original pos (.x coordinate works for some reason)
            if (a.transform.parent.position.x == s.originalPos.x)
                a.SetBool("run", false);

            // run to original position
            s.RunTowardsTarget(s.originalPos);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
