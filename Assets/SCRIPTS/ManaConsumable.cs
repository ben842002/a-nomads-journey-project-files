using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaConsumable : MonoBehaviour
{
    ManaBar manaBar;
    public int manaGain;
    public GameObject effect;

    PlayerStats stats;

    // Start is called before the first frame update
    void Start()
    {   
        manaBar = GameObject.FindGameObjectWithTag("PlayerManabar").GetComponent<ManaBar>();
        stats = PlayerStats.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stats.currentMana += manaGain;
            if (stats.currentMana > stats.maxMana)
                stats.currentMana = stats.maxMana;

            manaBar.SetMana(stats.currentMana);

            Effect();
            Destroy(gameObject);
        }
    }

    void Effect()
    {
        GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(_effect, 0.5f);
    }
}
