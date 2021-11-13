using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float movementForce;
    public float updatePathRepeatRate = 0.5f;

    // how close object needs to be to a waypoint before moving on to the next
    public float nextWaypointDistance;

    [Header("Attach if enemy has parent")]
    public Transform parent;

    [Header("Return to Original Position")]
    public bool returnToOrginalPos;
    public float returnOrigPosSpeed;

    [Header("Minimum Distance")]
    public bool setMinimumDist;
    public float minimumDistance;

    Path path; // current path that object is following
    int currentWaypoint; // store current waypoint along that path

    Seeker seeker;
    Rigidbody2D rb;

    Vector3 originalPos;
    Collider2D mainCollider;

    // ***MAKE SURE ALL ENEMIES USING PATHFINDING HAS ENEMYKNOCKBACK***
    EnemyKnockback enemyKB;

    Transform playerTarget;
    float nextTImeToSearch;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        mainCollider = GetComponent<Collider2D>();

        // check if rb is located on this object
        AccessRigidbody2D();

        // check if enemyKB is located on this object
        AccessEnemyKnockback();

        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(UpdatePath), 0f, updatePathRepeatRate);
    }

    void UpdatePath()
    {   
        if (seeker.IsDone() == true)
        {
            if (playerTarget != null)
            {
                // generate path
                seeker.StartPath(rb.position, playerTarget.position, OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path _path)
    {
        // if we didnt get any errors
        if (_path.error == false)
        {   
            // set current path to newly generated path AND reset progress on path
            path = _path;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (playerTarget == null)
        {
            ReturnOriginalPos();

            FindPlayer();
            return;
        }
        else if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)       
            return;      

        // move object
        // first we have to find the direction to the next waypoint, then move in that direction
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        
        // only move if enemy is not knocked back
        if (enemyKB.KnockBackTimer <= 0)
        {   
            // if bool is true, player has to be in range. If not, automatically move towards player
            if (setMinimumDist == true)
            {   
                // only move towards player if they are in range
                if (Vector2.Distance(rb.position, playerTarget.position) <= minimumDistance)
                {
                    // check if collider is a trigger
                    if (mainCollider.isTrigger == true)
                        mainCollider.isTrigger = false;

                    Vector2 force = direction * movementForce * Time.fixedDeltaTime;
                    rb.AddForce(force);
                }
                else
                {
                    if (returnToOrginalPos == true)
                        ReturnOriginalPos();
                }
            }
            else
            {
                Vector2 force = direction * movementForce * Time.fixedDeltaTime;
                rb.AddForce(force);
            }
        }
        else
        {   
            enemyKB.KnockBackTimer -= Time.fixedDeltaTime;
            enemyKB.EnemyKnockBack(enemyKB.knockFromRight, enemyKB.knockBackAmount, rb);
        }

        // find distance between next waypoint. If it is less than nWD, move onto next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void ReturnOriginalPos()
    {
        // avoid collisions going back to origin pos
        mainCollider.isTrigger = true;

        // if there is no parent gameobject for the enemy, move the SPRITE GAMEOBJECT
        if (parent == null)
        {
            // move back to original position
            enemyKB.transform.position = Vector3.MoveTowards(enemyKB.transform.position, originalPos,
                returnOrigPosSpeed * Time.fixedDeltaTime);
        }
        else
        {   
            // if there is a parent, move the PARENT so that ALL gameobjects of the enemy move in sync
            enemyKB.transform.parent.position = Vector3.MoveTowards(enemyKB.transform.parent.position, originalPos,
                returnOrigPosSpeed * Time.fixedDeltaTime);
        }

        if (enemyKB.transform.position == originalPos)
            mainCollider.isTrigger = false;
    }

    // -----------------------------------------
    // find RB and enemy KB
    void AccessRigidbody2D()
    {
        // check if enemyKB is located on this object
        if (TryGetComponent<Rigidbody2D>(out _) == true)
            rb = GetComponent<Rigidbody2D>();
        else
            rb = GetComponentInParent<Rigidbody2D>();
    }

    void AccessEnemyKnockback()
    {
        if (TryGetComponent<EnemyKnockback>(out _) == true)
        {
            enemyKB = GetComponent<EnemyKnockback>();
            originalPos = transform.position;
        }
        else
        {
            // access enemyKB from parent object
            enemyKB = GetComponentInParent<EnemyKnockback>();
            originalPos = transform.parent.position;
        }
    }

    // -------------------------------------------------

    void FindPlayer()
    {
        if (nextTImeToSearch <= Time.time)
        {
            GameObject sResult = GameObject.FindGameObjectWithTag("Player");
            if (sResult != null)
            {
                playerTarget = sResult.transform;

                if (mainCollider.isTrigger == true)
                    mainCollider.isTrigger = false;
            }
            nextTImeToSearch = Time.time + 0.75f;
        }
    }
}
