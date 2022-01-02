using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatsData
{
    public int levelIndex;

    // Health and Lives
    public int maxHealth;
    public int maxLives;

    // Mana 
    public int maxMana;
    public int enemyDeathMana;

    // Player learned abilities
    public bool heal;
    public bool dashAbility;
    public bool doubleJump;
    public bool bowAbility;

    public float moveSpeed;
    public float jumpSpeed;
    public float dashSpeed;

    // constructor (see C++ vids by conti for csc 211 for clarification)
    public PlayerStatsData(PlayerStats stats)
    {
        /* store PlayerStats values from GameObject in scene into this class's variables
         * some variables aren't included from playerstats because they are not needed (either wont change or are assigned 
         * from Start() in-game */
        //-----------------------------------
        levelIndex = stats.levelIndex;

        // hp and lives
        maxHealth = stats.maxHealth;
        maxLives = stats.maxLives;

        // mana
        maxMana = stats.maxMana;
        enemyDeathMana = stats.enemyDeathMana;

        // abilities
        heal = stats.heal;
        dashAbility = stats.dashAbility;
        doubleJump = stats.doubleJump;
        bowAbility = stats.bowAbility;

        moveSpeed = stats.moveSpeed;
        jumpSpeed = stats.jumpSpeed;
        dashSpeed = stats.dashSpeed;
    }
}
