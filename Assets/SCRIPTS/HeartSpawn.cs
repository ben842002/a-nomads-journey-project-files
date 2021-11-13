using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> heartsList;
    [SerializeField] int maxHearts;
    public GameObject heartsPrefab;
    [SerializeField] float timerC;
    float timer;

    // Update is called once per frame
    void Update()
    {   
        // check if list isnt capped
        if (heartsList.Count < maxHearts)
        {
            if (timer <= 0)
            {   
                // spawn heart and add element to list
                GameObject heart = Instantiate(heartsPrefab, transform.position, Quaternion.identity);
                heartsList.Add(heart);

                timer = timerC;
            }
            else
                timer -= Time.deltaTime;
        }

        //// delete empty elements (player picked up heart)
        for (int i = 0; i < heartsList.Count; i++)
        {
            if (heartsList[i] == null)
                heartsList.Remove(heartsList[i]);
        }
    }
}
