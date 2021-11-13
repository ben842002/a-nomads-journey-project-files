using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantHeal : MonoBehaviour
{
    Collider2D Collider;
    DialogueTrigger trigger;

    [SerializeField] GameObject itemEffect;

    private void Start()
    {
        Collider = GetComponent<Collider2D>();
        trigger = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats.instance.heal = true;
            SaveSystem.SavePlayerStats(FindObjectOfType<PlayerStats>());

            trigger.TriggerDialogue();
            Destroy(transform.parent.gameObject);

            // effect
            GameObject effect = Instantiate(itemEffect, transform.parent.position, transform.rotation);
            Destroy(effect, .5f);
        }          
    }

    // called on anim frame
    void EnableCollider()
    {
        Collider.enabled = true;
    }
}
