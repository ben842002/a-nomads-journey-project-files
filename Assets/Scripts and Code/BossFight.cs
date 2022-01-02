using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossFight : MonoBehaviour
{
    // remember to use arrays for FIXED LENGTH elements and use Lists for DYNAMIC 
    [Header("CINEMACHINE CAMERA")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("OPTIONAL: Boss Audio Track")]
    public bool hasMusicTrack;
    [Tooltip("ONLY FILL THIS OUT IF BOOL ABOVE IS TOGGLED TRUE")]
    public string musicName;
    bool playedMusic;

    // note for triggering boss music: it is called in the ontriggerenter2D method at the very bottom

    [Header("WALLS")]
    public float wallSpeed;
    public Transform[] walls;
    public Transform[] wallGoToPositions;
    public List<Vector3> wallsOriginalPositions;
    bool triggerMove = false;
    bool triggeredOnce = false; // only want to move the walls once

    [Header("ENEMIES")]
    [Tooltip("Some enemies will spawn once player enters area while others will already be active")]
    public bool spawnEnemies;  
    public List<GameObject> enemies;

    // called when fight is finished
    bool isDone;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < walls.Length; i++)
            wallsOriginalPositions.Add(walls[i].position);
    }

    // Update is called once per frame
    void Update()
    {   
        // move walls and spawn enemies if possible
        if (triggerMove == true && triggeredOnce == false)
        {
            //activate camera
            virtualCamera.m_Priority = 99;

            // true by default
            triggeredOnce = true;

            // move walls
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].position = Vector2.MoveTowards(walls[i].position, wallGoToPositions[i].position, wallSpeed * Time.deltaTime);

                // check if walls are at their designated location
                if (walls[i].position != wallGoToPositions[i].position)
                    triggeredOnce = false;
            }

            // spawn enemies if toggled
            if (spawnEnemies == true)
            {
                for (int i = 0; i < enemies.Count; i++)
                    enemies[i].SetActive(true);
            }
        }

        // move walls back once player has cleared all enemies
        if (enemies.Count == 0 && isDone == false)
        {
            // if there is a music track, turn it off once boss is dead
            if (hasMusicTrack == true)
                AudioManager.instance.Stop(musicName);

            // bool will be true by default
            isDone = true;

            // fight is done once the walls are back in their original pos
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].position = Vector2.MoveTowards(walls[i].position, wallsOriginalPositions[i], wallSpeed * Time.deltaTime);

                // check if walls are back in their pos
                if (walls[i].position != wallsOriginalPositions[i])
                    isDone = false;
            }

            // deactivate camera
            virtualCamera.m_Priority = 2;
        }
        else if (enemies.Count != 0)
        {
            // delete element when enemy is killed
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                    enemies.Remove(enemies[i]);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerMove = true;

            // play special audio boss track if enabled
            if (hasMusicTrack == true)
            {   
                // prevent from triggering the audio again
                if (playedMusic == false)
                {
                    AudioManager.instance.Play(musicName);
                    playedMusic = true;
                }
            }
        }
    }
}
