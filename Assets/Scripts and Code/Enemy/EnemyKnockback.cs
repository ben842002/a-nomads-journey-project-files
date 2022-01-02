using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    // REFERENCE THESE VALUES IN THE MOVEMENT SCRIPT OF THE ENEMY
    [Header("DONT CHANGE THESE VALUES")]
    public float KnockBackTimer;
    public bool knockFromRight;

    [Header("Knockback")]
    public float knockBackAmount;
    public float knockBackTimerCountdown;

    public void EnemyKnockBack(bool knockedFromRight, float knockbackForce, Rigidbody2D rb)
    {
        if (knockedFromRight == true)
            rb.velocity = new Vector2(-knockbackForce, 0f);
        else
            rb.velocity = new Vector2(knockbackForce, 0f);

    }
}
