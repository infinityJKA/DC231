//The entire purpose of this class is to hold the data for a room

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;
    public int type;
    //These store if there is a door in the corresponding direction
    public bool doorTop, doorBot, doorLeft, doorRight;


    //Default Constructor
    public Room(Vector2 _gridPos, int _type)
    {
        gridPos = _gridPos;
        type = _type;
    }

}