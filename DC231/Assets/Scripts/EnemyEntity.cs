using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    public int maxHP, atk, attackRange, minAttackRange, vision, score;  // attackRange and minAttackRange are both inclusive
    
    [HideInInspector] public int currentHP;
    public string enemyName;
    public bool canMove = true;

    public Tile enemyTile; // the tile the enemy is standing on

    public void InitializeStats(){
        currentHP = maxHP;
    }

    


}
