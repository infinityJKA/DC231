using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

//This script is used to generate a dungeon layout using a grid system. It creates rooms and sets their doors based on the positions of neighboring rooms.
//It is meant to emulate a dungeon layout similar to the one in the game "The Binding of Isaac".


public class DungeonGenerator : MonoBehaviour
{
    //this creates an 8x8 grid of potential rooms
    Vector2 gridSize = new Vector2(4, 4);
    Room[,] rooms;
    //using a list to be able to use the Contains method
    List<Vector2> takenPositions = new List<Vector2>();
    //made these public so they can be changed in the inspector
    public int gridSizeX, gridSizeY, roomCount = 20;
    public GameObject roomWhiteObj;

    private void Start()
    {
        //Error trapping if the # of rooms won't fit in the grid
        if (roomCount >= ((gridSize.x * 2) * (gridSize.y * 2)))
        {
            roomCount = Mathf.RoundToInt((gridSize.x * 2) * (gridSize.y) * 2);
        }

        //Set the grid size
        gridSizeX = Mathf.RoundToInt(gridSize.x);
        gridSizeY = Mathf.RoundToInt(gridSize.y);

        //Create the room array
        CreateRooms();
        SetRoomDoors();
        DrawMap();
    }

    void CreateRooms()
    {
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, 1);
        takenPositions.Insert(0, Vector2.zero);

        float randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
        Vector2 checkPos = Vector2.zero;

        for (int i = 0; i < roomCount - 1; i++)
        {
            float randomPerc = ((float)i) / ((float)roomCount - 1);
            float randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            checkPos = NewPosition();
            Debug.Log("Generated new position: " + checkPos);

            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
            {
                int iterations = 0;
                do
                {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);

                if (iterations >= 100)
                {
                    Debug.LogError("Error: could not create with fewer neighbors than: " + NumberOfNeighbors(checkPos, takenPositions));
                }
            }

            if (!takenPositions.Contains(checkPos) && checkPos.x < gridSizeX && checkPos.x >= -gridSizeX && checkPos.y < gridSizeY && checkPos.y >= -gridSizeY)
            {
                rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, 0);
                takenPositions.Insert(0, checkPos);
                Debug.Log("Placed room at: " + checkPos);
            }
            else
            {
                Debug.LogWarning("Invalid position: " + checkPos);
            }
        }
    }

    Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        int maxIterations = 1000; // Set a maximum iteration limit
        int iterations = 0;

        do
        {
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;

            bool UpDown = Random.value > 0.5f;
            bool LeftRight = Random.value > 0.5f;
            if (UpDown)
            {
                if (LeftRight) { y += 1; }
                else { y -= 1; }
            }
            else
            {
                if (LeftRight) { x += 1; }
                else { x -= 1; }
            }
            checkingPos = new Vector2(x, y);
            iterations++;
        } while ((takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY) && iterations < maxIterations);

        if (iterations >= maxIterations)
        {
            Debug.LogError("Error: Could not find a valid position within the iteration limit.");
        }

        return checkingPos;
    }

    Vector2 SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        int maxIterations = 1000; // Set a maximum iteration limit
        int iterations = 0;

        do
        {
            inc = 0;
            do
            {
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);

            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool UpDown = Random.value > 0.5f;
            bool LeftRight = Random.value > 0.5f;

            if (UpDown)
            {
                if (LeftRight) { y += 1; }
                else { y -= 1; }
            }
            else
            {
                if (LeftRight) { x += 1; }
                else { x -= 1; }
            }
            checkingPos = new Vector2(x, y);
            iterations++;
        } while ((takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY) && iterations < maxIterations);

        if (iterations >= maxIterations)
        {
            Debug.LogError("Error: Could not find a valid position within the iteration limit.");
        }

        return checkingPos;
    }

    int NumberOfNeighbors(Vector2 pos, List<Vector2> takenPositions)
    {
        //This checks how many neighbors a position has
        int neighbors = 0;
        if (takenPositions.Contains(new Vector2(pos.x + 1, pos.y))) { neighbors++; }
        if (takenPositions.Contains(new Vector2(pos.x - 1, pos.y))) { neighbors++; }
        if (takenPositions.Contains(new Vector2(pos.x, pos.y + 1))) { neighbors++; }
        if (takenPositions.Contains(new Vector2(pos.x, pos.y - 1))) { neighbors++; }
        return neighbors;
    }

    void SetRoomDoors()
    {
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                //Check if the room at the current position is null
                if (rooms[x, y] == null)
                { continue; }
                Vector2 gridPosition = new Vector2(x, y);

                //check above
                if (y - 1 < 0)
                { rooms[x, y].doorBot = false; }
                else
                { rooms[x, y].doorBot = rooms[x, y - 1] != null; }

                //check below
                if (y + 1 >= gridSizeY * 2)
                { rooms[x, y].doorTop = false; }
                else
                { rooms[x, y].doorTop = rooms[x, y + 1] != null; }

                //check left
                if (x - 1 < 0)
                { rooms[x, y].doorLeft = false; }
                else
                { rooms[x, y].doorLeft = rooms[x - 1, y] != null; }

                //check right
                if (x + 1 >= gridSizeX * 2)
                { rooms[x, y].doorRight = false; }
                else
                { rooms[x, y].doorRight = rooms[x + 1, y] != null; }
            }
        }
    }

    void DrawMap()
    {
        //This draws the map using the MapSpriteSelector script


        foreach (Room room in rooms)
        {
            if (room == null)
            { continue; }
            Vector2 drawPos = room.gridPos;
            //these are the dimensions of the rooms I created as a temp object, but they can be changed to whatever
            drawPos.x *= 16;
            drawPos.y *= 8;

            MapSpriteSelector mapper = Object.Instantiate(roomWhiteObj, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
            mapper.type = room.type;
            mapper.up = room.doorTop;
            mapper.down = room.doorBot;
            mapper.left = room.doorLeft;
            mapper.right = room.doorRight;
        }

    }

}