using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    /* This script reads a dialogue when the player interacts with the NPC. If toggled,
     * they will also spawn an item. This item will not spawn next time the player comes here
     * since there will be a save and load system.
    */

    [SerializeField] GameObject popupText;
    [SerializeField] DialogueTrigger regularDialogue;

    [Header("OPTIONAL: spawn item")]
    [SerializeField] bool spawnItem;
    [SerializeField] GameObject item;
    [SerializeField] string playerPrefsName;
    [SerializeField] DialogueTrigger postItemDialogue;

    Animator dialogueAnimator;
    Transform player;
    bool isInRange;

    float nextTimeToSearch;
    float localScale;

    // Start is called before the first frame update
    void Start()
    {
        dialogueAnimator = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        localScale = transform.localScale.x;

        // uncomment this line to reset playerprefs for testing purposes
        //PlayerPrefs.SetInt(playerPrefsName, 0);

        // check to see if player has already gotten the item. If so, remove unnecessary components
        int _spawnItem = PlayerPrefs.GetInt(playerPrefsName, 0);
        if (_spawnItem == 1)
        {
            Destroy(item);
            Destroy(regularDialogue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) && isInRange == true && dialogueAnimator.GetBool("isOpen") == false)
        {   
            if (spawnItem == true)
            {
                // CHECK IF PLAYER HAS ALREADY GOTTEN ITEM. If not, spawn the item
                if (PlayerPrefs.GetInt(playerPrefsName) == 0)
                {
                    regularDialogue.TriggerDialogue();
                    item.SetActive(true);
                    PlayerPrefs.SetInt(playerPrefsName, 1);
                }
                else
                {
                    // trigger post-item dialogue
                    postItemDialogue.TriggerDialogue();
                }
            }
            else
            {
                // trigger post-item dialogue
                regularDialogue.TriggerDialogue();
            }
        }

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector2(localScale, localScale);
        else
            transform.localScale = new Vector2(-localScale, localScale);
    }

    // THIS IS A DISCRETE CASE WHERE THE PLAYER DOESNT PICK UP AFTER TALKING TO NPC
    private void OnApplicationQuit()
    {
        GameObject healItem = GameObject.FindGameObjectWithTag("HealItem");
        if (healItem != null)
            PlayerPrefs.SetInt(playerPrefsName, 0);
    }

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject sResult = GameObject.FindGameObjectWithTag("Player");
            if (sResult != null)
                player = sResult.transform;
            nextTimeToSearch = Time.time + 0.75f;
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
        }
    }
}
