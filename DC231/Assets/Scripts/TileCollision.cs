using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollision : MonoBehaviour
{
    [SerializeField] public bool walkable;
    // Start is called before the first frame update
    public TileCollision(bool isWalkable)
    {
        walkable = isWalkable;
    }
}