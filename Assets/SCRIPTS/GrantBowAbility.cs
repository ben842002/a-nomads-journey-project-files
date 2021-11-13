using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantBowAbility : MonoBehaviour
{
    PlayerStats stats;
    DialogueTrigger trigger;

    public GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;
        trigger = GetComponent<DialogueTrigger>();

        if (stats.bowAbility == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stats.bowAbility = true;
            trigger.TriggerDialogue();

            GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(_effect, 0.5f);

            Destroy(gameObject);
        }
    }
}
