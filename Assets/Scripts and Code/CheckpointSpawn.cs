using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSpawn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameMaster.gm.checkPointPosition = transform.position;
    }
}
