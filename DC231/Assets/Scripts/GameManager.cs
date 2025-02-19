using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GridManager gridManager;
    [Header("Dungeon Data")]
    [SerializeField] FloorSettings floorSettings;

    [Header("Player Stuff")]
    [SerializeField] GameObject playerEntityPrefab;
    public GameObject playerEntity;
    public PlayerStats playerStats;
    public Tile playerTile;
    [SerializeField] GameObject cam;

    void Start(){
        gridManager.MakeGrid(floorSettings.floorLayout);
        playerEntity = Instantiate(playerEntityPrefab);
        playerTile = gridManager.GetTileAtPosition(0,0);
        playerTile.currentEntity = playerEntity;
        playerTile.MoveEntityToTile();
        SetCamToPlayer();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            PlayerMove(1,0);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)){
            PlayerMove(-1,0);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow)){
            PlayerMove(0,1);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            PlayerMove(0,-1);
        }
    }

    public void PlayerMove(int right, int up){
        Tile t = gridManager.GetTileAtPosition(playerTile.x+right,playerTile.y+up);
        if(t == null){
            Debug.Log("No tile to move to!");
        }
        else if (t.currentEntity != null){
            Debug.Log("An entity is already standing at "+playerTile.x+right+", "+playerTile.y+right+"!");
        }
        else{
            t.currentEntity = playerEntity;
            playerTile.currentEntity = null;
            playerTile = t;
            t.MoveEntityToTile();
            SetCamToPlayer();
        }
    }

    public void SetCamToPlayer(){
        cam.transform.position = new Vector3(playerEntity.transform.position.x,4,playerEntity.transform.position.z-5);
    }

}
