using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushroom_death : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator a, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mushroom m = a.GetComponent<Mushroom>();
        // always remember the .parent (you always forget!!! REEE)
        if (m.ambushScript != null)
            m.ambushScript.GetComponent<BossFight>().enemies.Remove(a.transform.parent.gameObject);

        Destroy(m);
        Destroy(a.GetComponent<Enemy>());

        GameObject canvasObj = a.transform.parent.GetComponentInChildren<Canvas>().gameObject;
        Destroy(canvasObj);

        a.gameObject.tag = "Untagged";
        a.gameObject.layer = 10;
        Physics2D.IgnoreLayerCollision(8, 10);
    }
}
