using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    public int maxHP, maxMP, atk, def, res;
    
    [HideInInspector] public int currentHP, currentMP;
    public string enemyName;

    public Tile enemyTile; // the tile the enemy is standing on

    public void InitializeStats(){
        currentHP = maxHP;
        currentMP = maxMP;
    }

    


}
