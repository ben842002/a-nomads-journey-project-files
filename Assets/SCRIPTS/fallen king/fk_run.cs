using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fk_run : StateMachineBehaviour
{
    FallenKing fk;
    FallenKingAttack fka;

    bool playerInRange;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fk = animator.GetComponent<FallenKing>();
        fka = animator.GetComponent<FallenKingAttack>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (fk.player == null)
            return;

        fk.FacePlayer();

        // check if player is in range for attack
        if (fk.facingRight == true)
            playerInRange = fka.PlayerInRangeForAttack(Vector2.right);
        else
            playerInRange = fka.PlayerInRangeForAttack(Vector2.left);

        // if player IS in range, randomize attack version. IF NOT, move towards player
        if (playerInRange == true)      
            fka.RandomizeAttack();       
        else       
            fk.MoveToPlayer();      
    }
}
