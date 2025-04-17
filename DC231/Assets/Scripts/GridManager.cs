using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public int gridWidth, gridHeight;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] GameObject wallEntityPrefab;
    public Dictionary<Vector2, Tile> tiles;

    void Start(){
        
    }


    public void MakeGridFromRooms(DungeonGeneratorV2 dungeon, GameManager gm){
        tiles = new Dictionary<Vector2, Tile>();
        foreach(RoomInstance room in dungeon.roomInstances){
            foreach(GameObject t in room.tiles){
                int x = (int)t.transform.position.x/16;
                int y = (int)t.transform.position.y/16;

                var spawnedTile = Instantiate(tilePrefab,new Vector3(x,y,0),Quaternion.identity);
                spawnedTile.name = $"Tile {x},{y}";
                //Debug.Log("spawning tile at "+x+","+y);

                //var isOffset = (x%2 == 0 && y%2 !=0) || (x%2 != 0 && y%2 == 0); // Sets offset colors for testing
                spawnedTile.Init();

                tiles[new Vector2(x,y)] = spawnedTile; // Adds the tile to dictionary for future reference  

                spawnedTile.gm = gm;   
                
                spawnedTile.x = x; // also store in tile itself for easier referencing
                spawnedTile.y = y;       

                spawnedTile.transform.parent = transform; // parent under the manager so it doesn't make the hierarchy look messy

                Sprite s = t.GetComponent<SpriteRenderer>().sprite;
                spawnedTile.spriteRenderer.sprite = s;

                if(t.GetComponent<TileCollision>() != null){
                    Debug.Log("Has TileCollision property");
                    if(t.GetComponent<TileCollision>().walkable == false){
                        Debug.Log("IS NOT WALKABLE");
                        GameObject w = SpawnEntity(wallEntityPrefab,x,y);
                        spawnedTile.currentEntity = w;
                        //w.transform.parent = spawnedTile.transform;
                    }
                }
            }
        }
    }

    public void MakeGrid(List<TileRowSpawnData> rows, GameManager gm){
        tiles = new Dictionary<Vector2, Tile>();
        gridHeight = rows.Count;
        gridWidth = 0;
        int y = 0;
        foreach(TileRowSpawnData row in rows){
            //Debug.Log("columnsInRow: "+row.columnsInRow.Count);
            if(row.columnsInRow.Count > gridWidth){gridWidth = row.columnsInRow.Count;}; // sets gridWidth
            int x = 0;
            foreach(TileColSpawnData col in row.columnsInRow){
                var spawnedTile = Instantiate(tilePrefab,new Vector3(x,y,0),Quaternion.identity);
                spawnedTile.name = $"Tile {x},{y}";

                var isOffset = (x%2 == 0 && y%2 !=0) || (x%2 != 0 && y%2 == 0); // Sets offset colors for testing
                spawnedTile.Init(isOffset);

                tiles[new Vector2(x,y)] = spawnedTile; // Adds the tile to dictionary for future reference  

                spawnedTile.gm = gm;   
                
                spawnedTile.x = x; // also store in tile itself for easier referencing
                spawnedTile.y = y;       

                spawnedTile.transform.parent = transform; // parent under the manager so it doesn't make the hierarchy look messy

                if(col.entity != null){
                    if(col.entity.GetComponent<EnemyEntity>() != false){
                        GameObject e = SpawnEntity(col.entity,x,y);
                        gm.enemiesAlive.Add(e);
                        e.GetComponent<EnemyEntity>().enemyTile = spawnedTile;
                        e.GetComponent<EnemyEntity>().InitializeStats();
                        spawnedTile.currentEntity = e;
                    }
                    else{
                        spawnedTile.currentEntity = SpawnEntity(col.entity,x,y);
                    }
                }

                x++;
            }
            y++;
        }
        Debug.Log("Grid Width: "+gridWidth);
        Debug.Log("Grid Height: "+gridHeight);
    }

    public GameObject SpawnEntity(GameObject g, int x, int y){
        g = Instantiate(g);
        Tile t = GetTileAtPosition(x,y);
        t.currentEntity = g;
        t.MoveEntityToTile();
        return g;
    }

    public void OldMakeGrid(){  // creates the grid (OUTDATED, this version does NOT use FloorSettings)
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
