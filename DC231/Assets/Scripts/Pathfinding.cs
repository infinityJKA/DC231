using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEditor.Rendering;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 10; // Both are equal, not based on real distance which would be 10-14

    private List<Tile> openList;
    private List<Tile> closedList;

    // Code based on https://www.youtube.com/watch?v=alU04hvz6L4 but modified to fit the preexisting code architecture
    public List<Tile> Astar_Pathfind(int startX, int startY, int endX, int endY, GridManager grid, PathfindingOption pfo){
        Tile startTile = grid.GetTileAtPosition(startX,startY);
        Tile endTile = grid.GetTileAtPosition(endX,endY);

        openList = new List<Tile>{startTile};             // tiles that haven't been checked
        closedList = new List<Tile>();                    // tiles that have been eliminated

        for(int x = 0; x < grid.gridWidth; x++){          // reset previous calculations
            for(int y = 0; y < grid.gridHeight; y++){
                Tile tile = grid.GetTileAtPosition(x,y);
                tile.gCost = int.MaxValue;
                tile.CalculateFCost();
                tile.pathfindingCameFrom = null;
            }
        }

        startTile.gCost = 0;  // initialize values for the starting node
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while(openList.Count > 0){ // Loops through each neighbor until reaching the target or failing
            Tile currentTile = GetLowestFCostTile(openList);
            if(currentTile == endTile){
                // This means it has reached the final node
                return CalculatePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            PrintTileList("Neighbors",GetNeighborList(currentTile, grid));
            // iterate through each neighbor
            foreach (Tile n in GetNeighborList(currentTile, grid)){
                if(closedList.Contains(n)){ //move onto the next neighbor if this neighbor has already been checked/used
                    //Debug.Log("Invalid tile, already part of closedList "+n.x+","+n.y);
                    continue;
                }
                if(n.currentEntity != null && pfo != PathfindingOption.AttackRange){ // move onto the next neighbor is something else is already standing here
                    if(pfo == PathfindingOption.EnemyMove && n == endTile){
                        //Debug.Log("Reached player!");
                    }
                    else{
                        //Debug.Log("Invalid tile, something already standing at "+n.x+","+n.y);
                        closedList.Add(n);
                        continue;
                    }
                };

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile,n);
                //Debug.Log("tenativeGCost "+tentativeGCost+"  <<vs>> neighbor gCost " + n.gCost);
                if(tentativeGCost < n.gCost){ // chooses the neighbor if it is more optimal
                    n.pathfindingCameFrom = currentTile;
                    n.gCost = tentativeGCost;
                    n.hCost = CalculateDistanceCost(n,endTile);
                    n.CalculateFCost();
                    // adds the neighbor to openList if not already added
                    if(!openList.Contains(n)){
                        openList.Add(n);
                    }
                }
            }
        }

        // if this was reached, it means there was no valid path
        Debug.Log("No valid path");
        return null;
    }

    private List<Tile> GetNeighborList(Tile currentTile, GridManager g){  // Returns a list of all neighboring tiles
        List<Tile> nl = new List<Tile>(); // Neighbor List

        if(currentTile.x-1 >= 0){
            // Left
            nl.Add(g.GetTileAtPosition(currentTile.x-1, currentTile.y));
            // Left Down
            if(currentTile.y-1 >= 0) nl.Add(g.GetTileAtPosition(currentTile.x-1, currentTile.y-1));
            // Left Up
            if(currentTile.y+1 < g.gridHeight) nl.Add(g.GetTileAtPosition(currentTile.x-1, currentTile.y+1));
        }
        if(currentTile.x + 1 < g.gridWidth){
            // Right
            nl.Add(g.GetTileAtPosition(currentTile.x+1, currentTile.y));
            // Right Down
            if(currentTile.y-1 >= 0) nl.Add(g.GetTileAtPosition(currentTile.x+1, currentTile.y-1));
            // Right Up
            if(currentTile.y+1 < g.gridHeight) nl.Add(g.GetTileAtPosition(currentTile.x+1, currentTile.y+1));
        }
        // Down
        if(currentTile.y-1 >= 0) nl.Add(g.GetTileAtPosition(currentTile.x, currentTile.y-1));
        // Up
        if(currentTile.y+1 < g.gridHeight) nl.Add(g.GetTileAtPosition(currentTile.x, currentTile.y+1));

        return nl;
    }

    private List<Tile> CalculatePath(Tile endTile){ // moves backwards from the final tile to calculate the path taken to go from the start to end
        List<Tile> path = new List<Tile>();
        path.Add(endTile);
        Tile currentTile = endTile;
        while(currentTile.pathfindingCameFrom != null){  // start tile doesn't have a cameFrom so it moves back until it reaches the start
            path.Add(currentTile.pathfindingCameFrom);
            currentTile = currentTile.pathfindingCameFrom;
        }
        path.Reverse(); // reverse bc the path started at the end tile

        return path;
    }

    private int CalculateDistanceCost(Tile a, Tile b){
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDist - yDist);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDist,yDist) + MOVE_STAIGHT_COST * remaining;
    }

    private Tile GetLowestFCostTile(List<Tile> tileList){  // Finds the tile with the lowest fCost in the list
        Tile lowestFCostTile = tileList[0];
        for(int i = 1; i < tileList.Count; i++){
            if(tileList[i].fCost < lowestFCostTile.fCost){
                lowestFCostTile = tileList[i];
            }
        }
        return lowestFCostTile;
    }

    public void PrintTileList(String title,List<Tile> path){
        if(path == null || path.Count == 0){
            Debug.Log("list is empty");
        }
        else{
            String s = title+": ";
            foreach(Tile t in path){
                s = s + "("+t.x+","+t.y+") ";
            }
            Debug.Log(s);
        }
    }
}

public enum PathfindingOption{
    EnemyMove, // Tiles with entities are invalid other than playerTile
    AttackRange // Ignores all entites, used to check if an attack can reach the player
}
