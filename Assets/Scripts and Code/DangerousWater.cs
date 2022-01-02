using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousWater : MonoBehaviour
{
    [SerializeField] float timerCountdown;
    float timer;

    [SerializeField] int damage;
    bool doDmg;

    // Start is called before the first frame update
    void Start()
    {
        timer = timerCountdown;
    }

    // Update is called once per frame
    void Update()
    {
        if (doDmg == true)
        {
            if (timer <= 0)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    player.GetComponent<Player>().DamagePlayer(damage);

                // reset timer 
                timer = timerCountdown;
            }
            else
                timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doDmg = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doDmg = false;
            timer = timerCountdown;
        }
    }
}
