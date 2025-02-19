using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileRowSpawnData // these are the rows being stored by FloorSetting
{
   public List<TileColSpawnData> columnsInRow; // each item in row is a column

}

[System.Serializable]
   public struct TileColSpawnData{ // these are the "columns" that make up each row
      public GameObject entity; // this is the entity that will start on this [col,row] location (enemy, chest, door, etc.)
   }
