using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fk_death : StateMachineBehaviour
{
    [SerializeField] float camShake;
    [SerializeField] float camTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CinemachineShake.instance.ShakeCamera(camShake, camTime);
        Destroy(animator.GetComponent<Enemy>());
        Destroy(animator.GetComponent<FallenKing>());
        Destroy(animator.GetComponent<FallenKingAttack>());
        Destroy(animator.GetComponent<FallenKingProjectiles>());

        // destroy canvas
        GameObject canvasObject = animator.transform.GetChild(2).gameObject;
        Destroy(canvasObject);

        animator.gameObject.tag = "Untagged";
        animator.gameObject.layer = 10;
        Physics2D.IgnoreLayerCollision(8, 10);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
