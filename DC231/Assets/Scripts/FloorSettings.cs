using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorSettings", menuName = "Dungeon Crawler/FloorSettings")]
public class FloorSettings : ScriptableObject
{
    public List<TileRowSpawnData> floorLayout;
}