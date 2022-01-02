using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectKeys : MonoBehaviour
{
    public List<GameObject> keysList;
    [SerializeField] bool has3keys;
    bool isInRange;

    [Header("Text")]
    [SerializeField] GameObject need3KeysText;
    [SerializeField] GameObject enterRoomText;
    [SerializeField] Text numberOfKeysText;

    private void Start()
    {
        enterRoomText.SetActive(false);
        need3KeysText.SetActive(false);
        numberOfKeysText.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // player needs 3 keys to enter room
        if (keysList.Count == 3)
        {
            if (Input.GetKeyDown(KeyCode.W) && isInRange == true)
            {
                int nextLvlIndex = SceneManager.GetActiveScene().buildIndex + 1;
                StartCoroutine(LoadLevel.instance.LoadLevelIndex(nextLvlIndex));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            // player can now enter the final room
            if (keysList.Count == 3)
            {
                isInRange = true;
                enterRoomText.SetActive(true);
            }
            else
            {
                need3KeysText.SetActive(true);

                // show amount of keys the player has collected
                numberOfKeysText.transform.parent.gameObject.SetActive(true);
                numberOfKeysText.text = keysList.Count + " / 3";
            }
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            enterRoomText.SetActive(false);
            need3KeysText.SetActive(false);
            numberOfKeysText.transform.parent.gameObject.SetActive(false);
        }
    }
}
