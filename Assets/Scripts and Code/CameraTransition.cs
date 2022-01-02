using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;
    
    [Header("Make sure value synchronizes with respawnDelay in GM")]
    [SerializeField]
    private float priorityDelay = 1f;

    [Header("GameOverUI")]
    public GameObject gameOverUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            cinemachineCamera.m_Priority = 11;
    }

    // Important thing to note about OnTriggerExit2D:
    // whenever a collider is disabled or destroyed, this method is called (caused logic issues with player dashing in camera range)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // check if collider2D is enabled and if so, immediately switch to other camera
            if (collision.enabled == true)
            {
                cinemachineCamera.m_Priority = 2;
            }
            // work around to jumping out of water in a transition zone (capsule col. is on and off)
            else if (collision is CapsuleCollider2D)
            {
                return;
            }
            else
            {
                // if not, wait a few seconds before switching back to regular cam
                // NOTE: when player dies (air or ground), the 2D collider is disabled and allows for camera shake from GameMaster
                // CAMERA AMP AND FREQ VALUES ARE RESET IN GAMEMASTER
                StartCoroutine(ResetPriority(cinemachineCamera));
            }
        }
    }

    IEnumerator ResetPriority(CinemachineVirtualCamera cinemachineCamera)
    {
        yield return new WaitForSeconds(priorityDelay);

        // if game ends, dont move screen because some cameras have specific confiners
        // dont let player see beyond what is given KEKW
        if (gameOverUI.activeSelf == false)
            cinemachineCamera.m_Priority = 2;
    }
}
