//This script is used to generate the room based on the texture and the door information.
//It uses a texture to determine which tiles to spawn and where to spawn them.
//It also handles the door placement and the room size.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomInstance : MonoBehaviour
{

    // stores tiles after spawned
    public List<GameObject> tiles;

    //stores the template data
    public Texture2D tex;
    [HideInInspector]

    //information on the room
    public Vector2 gridPos;
    public int type; // 0: normal, 1: enter
    [HideInInspector]
    public bool doorTop, doorBot, doorLeft, doorRight;
    [SerializeField]

    //prefabs for the doors and walls
    GameObject doorU, doorD, doorL, doorR, doorWall;
    [SerializeField]

    //Gathers information from the colors of the sprite sheet (Regular tile, wall, obstacle)
    public ColorToGameObject[] mappings;
    
    // the size of the tiles and the room in tiles
    float tileSize = 16;
    Vector2 roomSizeInTiles = new Vector2(9, 17);


    //this is replacing the start method
    public void Setup(Texture2D _tex, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft, bool _doorRight)
    {
        tex = _tex;
        gridPos = _gridPos;
        type = _type;
        doorTop = _doorTop;
        doorBot = _doorBot;
        doorLeft = _doorLeft;
        doorRight = _doorRight;
        MakeDoors();
        GenerateRoomTiles();
    }
    //places the doors/walls in the room
    void MakeDoors()
    {
        //top door, get position then spawn
        Vector3 spawnPos = transform.position + Vector3.up * (roomSizeInTiles.y / 4 * tileSize) - Vector3.up * (tileSize / 4);
        PlaceDoor(spawnPos, doorTop, doorU);
        //bottom door
        spawnPos = transform.position + Vector3.down * (roomSizeInTiles.y / 4 * tileSize) - Vector3.down * (tileSize / 4);
        PlaceDoor(spawnPos, doorBot, doorD);
        //right door
        spawnPos = transform.position + Vector3.right * (roomSizeInTiles.x * tileSize) - Vector3.right * (tileSize);
        PlaceDoor(spawnPos, doorRight, doorR);
        //left door
        spawnPos = transform.position + Vector3.left * (roomSizeInTiles.x * tileSize) - Vector3.left * (tileSize);
        PlaceDoor(spawnPos, doorLeft, doorL);
    }

    //places the door or wall at the given position
    void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorSpawn)
    {
        // Check whether it's a door or wall, then spawn
        GameObject t = Instantiate(door ? doorSpawn : doorWall, spawnPos, Quaternion.identity);
        t.transform.parent = transform;

        // Assign the appropriate sprite
        SpriteRenderer spriteRenderer = t.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (door)
            {
                spriteRenderer.sprite = PlayerStats.instance.biomes[PlayerStats.instance.biomeIndex].floor;
            }
            else
            {
                spriteRenderer.sprite = PlayerStats.instance.biomes[PlayerStats.instance.biomeIndex].wall;
            }
        }

        tiles.Add(t);
    }


    //this method generates the room tiles based on the texture
    void GenerateRoomTiles()
    {
        //loop through every pixel of the texture
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }


    //this method generates the tile at the given position
    void GenerateTile(int x, int y)
    {
        //gets the color of the pixel
        Color pixelColor = tex.GetPixel(x, y);
        
        //skip clear spaces in texture
        if (pixelColor.a == 0)
        {
            return;
        }

        //find the color to match the pixel
        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color.Equals(pixelColor))
            {

                Vector3 spawnPos = positionFromTileGrid(x, y-4);
                GameObject t = Instantiate(mapping.prefab, spawnPos, Quaternion.identity);

                SpriteRenderer s = t.GetComponent<SpriteRenderer>();

                if(mapping.objType == RoomObjectType.Floor)
                {
                    s.sprite = PlayerStats.instance.biomes[PlayerStats.instance.biomeIndex].floor;
                }
                else if(mapping.objType == RoomObjectType.Wall)
                {
                    s.sprite = PlayerStats.instance.biomes[PlayerStats.instance.biomeIndex].wall;
                }
                else if(mapping.objType == RoomObjectType.Obstacle)
                {
                    s.sprite = PlayerStats.instance.biomes[PlayerStats.instance.biomeIndex].obstacle;
                }

                
                t.transform.parent = this.transform;
                tiles.Add(t);
               
            }
        }
    }

    //this method converts the tile grid position to world position
    Vector3 positionFromTileGrid(int x, int y)
    {
        Vector3 ret;
        //find difference between the corner of the texture and the center of this object
        Vector3 offset = new Vector3((-roomSizeInTiles.x + 1-4) * tileSize, (roomSizeInTiles.y / 4) * tileSize - (tileSize / 4), 0);
        //find scaled up position at the offset
        ret = new Vector3(tileSize * (float)x, -tileSize * (float)y, 0) + offset + transform.position;
        return ret;
    }
}

