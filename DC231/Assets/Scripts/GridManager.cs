using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    //public int gridWidth, gridHeight;
    public int gridMinX,gridMaxX,gridMinY,gridMaxY;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Chest chestPrefab;
    [SerializeField] GameObject wallEntityPrefab, floorExitPrefab;
    public Dictionary<Vector2, Tile> tiles;

    void Start(){
        
    }


    public void MakeGridFromRooms(DungeonGeneratorV2 dungeon, GameManager gm){
        tiles = new Dictionary<Vector2, Tile>();
        gridMinX = gridMaxX = gridMinY = gridMaxY = 0;
        foreach(RoomInstance room in dungeon.roomInstances){
            foreach(GameObject t in room.tiles){
                int x = (int)t.transform.position.x/16;
                int y = (int)t.transform.position.y/16;

                if(x > gridMaxX){gridMaxX = x;}
                if(x < gridMinX){gridMinX = x;}
                if(y > gridMaxY){gridMaxY = y;}
                if(y < gridMinY){gridMinY = y;}


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

        // destory all children (old tiles)
        foreach(Transform t in dungeon.transform){
            Destroy(t.gameObject);
        }

        // gets enemies/items of the correct floor
        List<EnemyEntity> toSpawn = gm.enemies1;
        List<Item> chestItems = gm.items1;
        int floor = PlayerStats.instance.dungeonFloor;
        
        if(floor <= 2){   // list1 floors 1-2
            Debug.Log("Floor is LEVEL 1");
            // already set as default
        }
        else if(floor <=5){ // list2 floors 3-5
            Debug.Log("Floor is LEVEL 2");
            toSpawn = gm.enemies2;
            chestItems = gm.items2;
        }
        else if(floor <=8){ // list3 floors 6-8
            Debug.Log("Floor is LEVEL 3");
            toSpawn = gm.enemies3;
            chestItems = gm.items3;
        }
        else if(floor <=12){ // list4 floors 9-13
            Debug.Log("Floor is LEVEL 4");
            toSpawn = gm.enemies4;
            chestItems = gm.items4;
        }
        else if(floor <=16){ // list5 floors 13-16
            Debug.Log("Floor is LEVEL 5");
            toSpawn = gm.enemies5;
            chestItems = gm.items5;
        }
        else if(floor <=21){ // list6 floors 17-21
            Debug.Log("Floor is LEVEL 6");
            toSpawn = gm.enemies6;
            chestItems = gm.items6;
        }
        else if(floor <=27){ // list7 floors 22-27
            Debug.Log("Floor is LEVEL 7");
            toSpawn = gm.enemies7;
            chestItems = gm.items7;
        }


        // spawn enemies
        bool isSpawned = false;
        int enemiesToSpawn = 13;
        for(int i = 0; i < enemiesToSpawn; i++){
            isSpawned = false;
            while(!isSpawned){
                Tile t = tiles.ElementAt(Random.Range(0,tiles.Count-1)).Value;
                if(t.currentEntity == null && t != gm.playerTile){
                    GameObject e = SpawnEntity(toSpawn[Random.Range(0,toSpawn.Count)].gameObject,t.x,t.y);
                    gm.enemiesAlive.Add(e);
                    e.GetComponent<EnemyEntity>().enemyTile = t;
                    e.GetComponent<EnemyEntity>().InitializeStats();
                    t.currentEntity = e;
                    isSpawned = true;
                }
            }
        }


        // spawn chests
        int chestsToSpawn = 10;
        for(int i = 0; i < chestsToSpawn; i++){
            isSpawned = false;
            while(!isSpawned){
                Tile t = tiles.ElementAt(Random.Range(0,tiles.Count-1)).Value;
                if(t.currentEntity == null && t != gm.playerTile){
                    var chest = Instantiate(chestPrefab,new Vector2(t.x,t.y),Quaternion.identity);
                    chest.item = chestItems[Random.Range(0,chestItems.Count)];
                    t.currentEntity = chest.gameObject;
                    isSpawned = true;
                }
            }
        }



        // spawn exits
        for(int i = 0; i < 2; i++){  // current will spawn 2 exits per floor
            isSpawned = false;
            while(!isSpawned){
                Tile t = tiles.ElementAt(Random.Range(0,tiles.Count-1)).Value;
                if(t.currentEntity == null && t != gm.playerTile){
                    GameObject e = SpawnEntity(floorExitPrefab,t.x,t.y);
                    t.currentEntity = e;
                    isSpawned = true;
                }
            }
        }


    }

    public void MakeGrid(List<TileRowSpawnData> rows, GameManager gm){
        tiles = new Dictionary<Vector2, Tile>();
        //gridHeight = rows.Count;
        //gridWidth = 0;
        int y = 0;
        foreach(TileRowSpawnData row in rows){
            //Debug.Log("columnsInRow: "+row.columnsInRow.Count);
            // if(row.columnsInRow.Count > gridWidth){gridWidth = row.columnsInRow.Count;}; // sets gridWidth
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
        //Debug.Log("Grid Width: "+gridWidth);
        //Debug.Log("Grid Height: "+gridHeight);
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
        // for(int x = 0; x < gridWidth; x++){
            // for(int z= 0; z < gridHeight; z++){
            //     var spawnedTile = Instantiate(tilePrefab,new Vector3(x,0,z),Quaternion.identity);
            //     spawnedTile.name = $"Tile {x},{z}";

            //     var isOffset = (x%2 == 0 && z%2 !=0) || (x%2 != 0 && z%2 == 0); // Sets offset colors for testing
            //     spawnedTile.Init(isOffset);

            //     tiles[new Vector2(x,z)] = spawnedTile; // Adds the tile to dictionary for future reference     
                
            //     spawnedTile.x = x; // also store in tile itself for easier referencing
            //     spawnedTile.y = z;       

            //     spawnedTile.transform.parent = transform; // parent under the manager so it doesn't make the hierarchy look messy

            // }
        // }
    }


    public Tile GetTileAtPosition(int posX, int posY){  // Find the tile at a specific coordinate
        if(tiles.TryGetValue(new Vector2(posX,posY), out var tile)){
            return tile;
        }
        Debug.Log("No tile found at pos "+posX+", "+posY);
        return null;
    }

    public void ResetCalc(){
        foreach(var item in tiles){
            item.Value.gCost = int.MaxValue;
            item.Value.CalculateFCost();
            item.Value.pathfindingCameFrom = null;
        }
    }



}
