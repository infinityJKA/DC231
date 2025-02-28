using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GridManager gridManager;
    public ControlState controlState;
    [Header("Dungeon Data")]
    [SerializeField] FloorSettings floorSettings;
    public List<GameObject> enemiesAlive;
    public int currentEnemyTurn = 0; // the index of the enemy that is currently taking its turn; 

    [Header("Player Stuff")]
    [SerializeField] GameObject playerObjectPrefab;
    public GameObject playerObject;
    public PlayerStats playerStats;
    public Tile playerTile;
    [SerializeField] GameObject cam;

    void Start(){
        enemiesAlive.Clear();
        gridManager.MakeGrid(floorSettings.floorLayout,this);
        playerObject = Instantiate(playerObjectPrefab);
        playerTile = gridManager.GetTileAtPosition(0,0);
        playerTile.currentEntity = playerObject;
        playerTile.MoveEntityToTile();
        SetCamToPlayer();
        controlState = ControlState.Player;
    }

    void Update(){
        if(controlState == ControlState.Player){ // checks if player can act
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
    }

    private void PlayerTookAction(){  // this is called after the player acts
        controlState = ControlState.Enemy;
        if(enemiesAlive.Count > 0){
            currentEnemyTurn = 0;
            StartCoroutine(EnemyTakeTurn(0.5f));
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
            t.currentEntity = playerObject;
            playerTile.currentEntity = null;
            playerTile = t;
            t.MoveEntityToTile();
            SetCamToPlayer();
            PlayerTookAction();
        }
    }

    public void SetCamToPlayer(){
        cam.transform.position = new Vector3(playerObject.transform.position.x,4,playerObject.transform.position.z-5);
    }

    public bool CheckIfTileIsValidWalk(int right, int up, Tile tile){  // Used in determining enemy movement
        Tile t = gridManager.GetTileAtPosition(tile.x+right,tile.y+up);
        if(t == null){
            Debug.Log("No tile to move to!");
        }
        else if (t.currentEntity != null){
            Debug.Log("An entity is already standing at "+playerTile.x+right+", "+playerTile.y+right+"!");
        }
        else{
            return true;
        }
        return false;
    }

    public Tile TileAddCord(int x, int y, Tile tile){  // Returns the tile (x,y) distance away from the given tile
        return gridManager.GetTileAtPosition(tile.x+x,tile.y+y);
    }


    private IEnumerator EnemyTakeTurn(float delay){ // recursively(?) called for each enemy

        yield return new WaitForSeconds(delay); // causes the delay between enemy turns

        GameObject obj = enemiesAlive[currentEnemyTurn];

        // Check attack range and attack if valid here (not implemented yet)
        if(false){
            ////////////////////////////////////////////////////////////////////
            //Debug.Log("HOW");
        }
        // Check for valid walkable tiles if can't attack
        else{
            List<Tile> movementOptions = new List<Tile>();
            //Debug.Log(movementOptions.Count);
            Tile originalTile = obj.GetComponent<EnemyEntity>().enemyTile;
            if(CheckIfTileIsValidWalk(1,0,originalTile)){movementOptions.Add(TileAddCord(1,0,originalTile));}
            if(CheckIfTileIsValidWalk(-1,0,originalTile)){movementOptions.Add(TileAddCord(-1,0,originalTile));}
            if(CheckIfTileIsValidWalk(0,1,originalTile)){movementOptions.Add(TileAddCord(0,1,originalTile));}
            if(CheckIfTileIsValidWalk(0,-1,originalTile)){movementOptions.Add(TileAddCord(0,-1,originalTile));}
            Debug.Log("Movement options: "+movementOptions.Count);

            if(movementOptions.Count > 0){
                int i = Random.Range(0,movementOptions.Count);
                Tile t = movementOptions[i]; // chooses random option out of valid tiles to move onto
                Debug.Log("Tile: "+t.x+","+t.y);
                obj.GetComponent<EnemyEntity>().enemyTile.currentEntity = null; // removes enemy from the original tile
                t.currentEntity = obj;  // sets the enemy to the new tile
                t.MoveEntityToTile();  // physically moves the gameobject over the new tile
                obj.GetComponent<EnemyEntity>().enemyTile = t;
                Debug.Log("Enemy moved!");
            }
            else{
                Debug.Log("Enemy couldn't move!");
            }
        }
        
        currentEnemyTurn++;
        if(currentEnemyTurn >= enemiesAlive.Count){
            controlState = ControlState.Player;
            Debug.Log("Player turn!");
        }
        else{
            StartCoroutine(EnemyTakeTurn(0.5f));
        }
    }

}

public enum ControlState{
    Player, Enemy
}
