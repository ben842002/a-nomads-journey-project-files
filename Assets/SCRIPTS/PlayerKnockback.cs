using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public static PlayerKnockback instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void KnockBackPlayer(Collider2D player, GameObject enemy)
    {
        // access player components to enable knockback
        PlayerMovement _player = player.GetComponent<PlayerMovement>();

        // discrete cases
        CheckForDiscreteCases(_player);

        // access player components to enable knockback
        _player.knockBackTimer = _player.knockBackTimerCountdown;

        // determine if enemy is on the left or right of player
        if (player.transform.position.x < enemy.transform.position.x)
            _player.knockedFromRight = true;
        else
            _player.knockedFromRight = false;
    }

    // put all if cases here
    void CheckForDiscreteCases(PlayerMovement _player)
    {
        if (_player.isJumping == true)
            _player.isJumping = false;
    }
}
