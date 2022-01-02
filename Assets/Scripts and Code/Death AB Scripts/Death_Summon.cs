using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Summon : MonoBehaviour
{
    CinemachineShake shake;

    [Header("Cam Shake")]
    [SerializeField] float camIntensity;
    [SerializeField] float camTime;

    [Header("Vertical")]
    [SerializeField] Transform[] summonSpawnVerticle;
    [SerializeField] GameObject verticalProjectilePrefab;

    [Header("Horizontal")]
    [SerializeField] Transform[] summonSpawnHorizontalLeft;
    [SerializeField] Transform[] summonSpawnHorizontalRight;
    [SerializeField] GameObject horizontalProjectilePrefab;


    // Start is called before the first frame update
    void Start()
    {
        shake = CinemachineShake.instance;
    }

    public void ChooseVerticalOrHorizontal()
    {
        AudioManager.instance.Play("DeathProjectiles");

        // choose randomly between horiz and vertical
        int random = Random.Range(0, 2);

        // get practice with switch statements 
        switch (random) 
        {   
            // vertical
            case 0:
                SummonProjectilePattern(summonSpawnVerticle, verticalProjectilePrefab, false);
                break;

            // horizontal
            case 1:
                // choose between left or right side
                int num = Random.Range(0, 2);
                if (num == 0)
                    SummonProjectilePattern(summonSpawnHorizontalLeft, horizontalProjectilePrefab, true);
                else
                    SummonProjectilePattern(summonSpawnHorizontalRight, horizontalProjectilePrefab, false);

                break;
        }
    }

    public void SummonProjectilePattern(Transform[] summonSpawnDirection, GameObject projectilePrefab, bool FlipRBandLS)
    {   
        // shake camera
        shake.ShakeCamera(camIntensity, camTime);

        // leave spot for player to dodge projectiles
        int indexToNotSpawn = Random.Range(0, summonSpawnDirection.Length - 1);
        for (int i = 0; i < summonSpawnDirection.Length; i++)
        {   
            // spawn projectiles in given positions
            if (i != indexToNotSpawn && i != indexToNotSpawn + 1)
            {   
                // cache in variable so possibly could change components
                GameObject proj = Instantiate(projectilePrefab, summonSpawnDirection[i].position, projectilePrefab.transform.rotation);

                // this code only applies for horizontal projectiles
                if (FlipRBandLS == true)
                {   
                    // flip components when proj is spawned on the left (its supposed to move right)
                    proj.transform.localScale = new Vector3(-1, 1, 1);
                    proj.GetComponent<Death_Horizontal_Projectile>().speed *= -1;
                }
            }
        }
    }
}
