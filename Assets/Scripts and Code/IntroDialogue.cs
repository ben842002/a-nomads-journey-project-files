using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    DialogueTrigger trigger;
    bool triggeredDialogue = false;

    /// <summary>
    /// You can't call trigger.TriggerDialogue() in Start because DialogueManager isn't referenced yet
    /// </summary>
    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && triggeredDialogue == false)
        {
            triggeredDialogue = true;
            trigger.TriggerDialogue();
        }
    }
}
