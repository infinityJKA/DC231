using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSpawnData
{
   [System.Serializable]
   public struct rowData{
      public GameObject entity;
   }

   public List<rowData> rows;

}
