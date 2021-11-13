using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    CollectKeys ck;
    public GameObject effectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ck = FindObjectOfType<CollectKeys>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            // add this key to the list 
            ck.keysList.Add(gameObject);

            gameObject.SetActive(false);
            Effect();
        }
    }

    void Effect()
    {
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 0.5f);
    }
}
