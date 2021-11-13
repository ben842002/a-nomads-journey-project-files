using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask playerMask;

    [Header("Overlap Box Components")]
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 size;

    private void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    // called on anim frame
    void DamagePlayer()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position + offset, size, 0f, playerMask);
        if (player != null)
        {   
            // damage player
            player.GetComponent<Player>().DamagePlayer(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position + offset, size);
    }
}
