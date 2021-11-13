using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenKingProjectiles : MonoBehaviour
{
    Animator anim;
    FallenKing fk;

    [Header("General Stuff")]
    [SerializeField] float timerCountdown;
    float timer;
    [SerializeField] Collider2D[] antiSpawnColliders;
    [SerializeField] int maxProjectiles;
    [SerializeField] float maxDelay;

    [Header("Coordinates")]
    [SerializeField] float upperYCord;
    [SerializeField] float lowerYCord;
    [SerializeField] float leftXCord;
    [SerializeField] float rightXCord;

    [Header("Linear Projectiles")]
    [SerializeField] GameObject linearProjectilePrefab;

    [Header("Homing Missile")]
    [SerializeField] GameObject homingMissilePrefab;

    [Header("Whirlwind")]
    [SerializeField] GameObject whirlwindPrefab; // whirlwind code is at the bottom
    [SerializeField] Vector3 playerPositionOffset;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        fk = GetComponent<FallenKing>();

        timer = timerCountdown;
    }

    private void Update()
    {   
        if (timer <= 0)
        {   
            // pick random amount of projectiles in given range
            int randomMax = Random.Range(1, maxProjectiles + 1);
            for (int i = 1; i <= randomMax; i++)
            {
                // choose randomly between linear and homing (0 and 1), index 2 is whirlwind)
                int random = Random.Range(0, 3);
                if (random == 0)
                    CheckIfSpawnPosValid(linearProjectilePrefab);
                else if (random == 1)
                    CheckIfSpawnPosValid(homingMissilePrefab);
                else
                    SpawnWhirlwind();
            }

            // reset timer
            timer = timerCountdown;
        }
        else
        {
            // countdown timer
            timer -= Time.deltaTime;
        }
    }

    void CheckIfSpawnPosValid(GameObject projectilePrefab)
    {   
        // generate a random spawn position
        float randomX = Random.Range(leftXCord, rightXCord);
        float randomY = Random.Range(lowerYCord, upperYCord);
        Vector2 spawn = new Vector2(randomX, randomY);

        // check if it is a valid pos the projectile can spawn in
        bool canSpawn = CanSpawnHere(spawn);
        if (canSpawn == true)
        {
            // spawn projectile at a random interval
            float randomDelay = Random.Range(0, maxDelay);
            StartCoroutine(SpawnProjectile(spawn, randomDelay, projectilePrefab));
        }
        else
        {
            // use recursion until a valid position is found
            CheckIfSpawnPosValid(projectilePrefab);
        }
    }

    IEnumerator SpawnProjectile(Vector3 spawn, float randomDelay, GameObject projectilePrefab)
    {
        yield return new WaitForSeconds(randomDelay);
        AudioManager.instance.Play("FKSummon");
        anim.ResetTrigger("Cast");
        anim.SetTrigger("Projectile");
        Instantiate(projectilePrefab, spawn, projectilePrefab.transform.rotation);
    }

    bool CanSpawnHere(Vector2 spawnPos)
    {
        for (int i = 0; i < antiSpawnColliders.Length; i++)
        {
            Vector3 centerPoint = antiSpawnColliders[i].bounds.center;
            float width = antiSpawnColliders[i].bounds.extents.x;
            float height = antiSpawnColliders[i].bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float upperExtent = centerPoint.y + height;
            float lowerExtent = centerPoint.y - height;

            // check if position is in a collider zone
            if (spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
            {
                if (spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                    return false;
            }
        }

        // return true by default
        return true;
    }

    // ------------------------------------
    // whirlwind

    void SpawnWhirlwind()
    {
        AudioManager.instance.Play("FKSummon");
        anim.ResetTrigger("Projectile");
        anim.SetTrigger("Cast");
        Instantiate(whirlwindPrefab, fk.player.position + playerPositionOffset, Quaternion.identity);
    }
}
