using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    [SerializeField]
    private float spawnDelayMax;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private float spawnTimerCountdown;
    float spawnTimer;

    [Header("Projectile Spawn Area")]
    [SerializeField] float leftXCord;
    [SerializeField] float rightXCord;
    [SerializeField] float upperYCord;
    [SerializeField] float lowerYCord;

    [Header("Max Projectiles In Scene")]
    [SerializeField] int numOfProjectiles;
    [SerializeField] List<GameObject> projectiles;

    private void Start()
    {
        spawnTimer = spawnTimerCountdown;
    }

    // Update is called once per frame
    void Update()
    {   
        // remove empty elements
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (projectiles[i] == null)
                projectiles.Remove(projectiles[i]);
        }

        if (spawnTimer <= 0)
        {   
            // 50% chance to spawn projectile
            int num = Random.Range(0, 2);
            if (num == 1)
            {   
                // make sure the # of projectiles doesnt go past the cap
                if (projectiles.Count < numOfProjectiles)
                {
                    // pick random location from given points
                    float randomX = Random.Range(leftXCord, rightXCord);
                    float randomY = Random.Range(lowerYCord, upperYCord);
                    Vector2 spawnPos = new Vector2(randomX, randomY);

                    // give projectiles a certain delay value
                    float spawnDelay = Random.Range(0f, spawnDelayMax);
                    StartCoroutine(SpawnProjectile(spawnDelay, spawnPos));
                }
            }
            
            // reset timer
            spawnTimer = spawnTimerCountdown;
        }
        else
            spawnTimer -= Time.deltaTime;
    }

    IEnumerator SpawnProjectile(float _spawnDelay, Vector2 spawnPos)
    {
        yield return new WaitForSeconds(_spawnDelay);

        // spawn projectile
        GameObject _projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // add the proj to list
        projectiles.Add(_projectile);
    }
}
