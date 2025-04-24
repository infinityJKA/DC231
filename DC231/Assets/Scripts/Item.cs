using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    
    
    

    [Header("Only gameplay")]
    public TileBase tile;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(1, 1); // inclusive, [0] in min and [1] is max
    public int atkModif;

    [Header("Only UI")]
    public bool stackable = true;
    public String itemName;

    [Header("Both")]
    public Sprite image;
}

public enum ItemType
{
    Weapon,
    Consumable
}

public enum  ActionType
{
    Swing,
    Shoot,
    Use
}
