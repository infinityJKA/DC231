using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth, gridHeight;
    [SerializeField] private Tile tilePrefab;
    private Dictionary<Vector2, Tile> tiles;

    void Start(){
        
    }

    public void MakeGrid(){  // creates the grid
        tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < gridWidth; x++){
            for(int z= 0; z < gridHeight; z++){
                var spawnedTile = Instantiate(tilePrefab,new Vector3(x,0,z),Quaternion.identity);
                spawnedTile.name = $"Tile {x},{z}";

                var isOffset = (x%2 == 0 && z%2 !=0) || (x%2 != 0 && z%2 == 0); // Sets offset colors for testing
                spawnedTile.Init(isOffset);

                tiles[new Vector2(x,z)] = spawnedTile; // Adds the tile to dictionary for future reference     
                
                spawnedTile.x = x; // also store in tile itself for easier referencing
                spawnedTile.y = z;       

                spawnedTile.transform.parent = transform; // parent under the manager so it doesn't make the hierarchy look messy

            }
        }
    }


    public Tile GetTileAtPosition(int posX, int posY){  // Find the tile at a specific coordinate
        if(tiles.TryGetValue(new Vector2(posX,posY), out var tile)){
            return tile;
        }
        Debug.Log("No tile found at pos "+posX+", "+posY);
        return null;
    }



}
