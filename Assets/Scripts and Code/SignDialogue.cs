using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignDialogue : MonoBehaviour
{
    DialogueTrigger trigger;
    Animator dialogueAnimator;

    bool isInRange = false;

    public GameObject popupText;

    // Start is called before the first frame update
    void Start()
    {   
        trigger = GetComponent<DialogueTrigger>();
        
        // DisplayNextSentence() script is attached to the dialogue box
        dialogueAnimator = FindObjectOfType<DisplayNextSentence>().GetComponent<Animator>();

        if (popupText.activeSelf == true)
            popupText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange == true && Input.GetKeyDown(KeyCode.W))
        {
            trigger.TriggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            popupText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            popupText.SetActive(false);
            dialogueAnimator.SetBool("isOpen", false);
        }
    }
}
