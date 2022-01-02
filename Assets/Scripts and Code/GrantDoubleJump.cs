using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantDoubleJump : MonoBehaviour
{
    bool isInRange;

    public GameObject effectPrefab;
    public GameObject pickupText;

    // Start is called before the first frame update
    void Start()
    {
        // comment for testing purposes (items dont get destroyed)
        CheckIfPlayerPickedUp();

        if (pickupText.activeSelf == true)
            pickupText.SetActive(false);
    }

    void CheckIfPlayerPickedUp()
    {
        // prevent from spawning again after player picks up
        bool hasAbility = PlayerStats.instance.doubleJump;
        if (hasAbility == true)
        {
            //player has picked up item, dont need to spawn
            Destroy(gameObject);
            Destroy(pickupText);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isInRange == true)
        {
            PlayerStats.instance.doubleJump = true;
            GetComponent<DialogueTrigger>().TriggerDialogue();  // give instructions on how to use

            GameObject effects = Instantiate(effectPrefab, transform.position, transform.rotation);
            Destroy(effects, 0.5f);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupText.SetActive(true);
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupText.SetActive(false);
            isInRange = false;
        }
    }
}
