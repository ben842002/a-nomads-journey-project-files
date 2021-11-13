using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushroom_run : StateMachineBehaviour
{
    Mushroom m;
    bool playerInRangeForAttack;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m = animator.GetComponent<Mushroom>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator a, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m.player == null)
            return;

        // face player
        m.FaceObject(m.player.position);

        // check if mushroom can attack
        if (m.facingRight)
            playerInRangeForAttack = m.CheckForPlayer(Vector2.right);
        else
            playerInRangeForAttack = m.CheckForPlayer(Vector2.left);

        if (playerInRangeForAttack == true)
        {
            // player is in range, so attack
            m.Attack();
        }
        else
        {
            // run to player
            m.MoveTowardsObject(m.player.position);
        }
    }
}
