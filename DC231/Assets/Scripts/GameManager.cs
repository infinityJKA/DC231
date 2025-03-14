using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GridManager gridManager;
    private Pathfinding pathfinding;
    public ControlState controlState;
    [Header("Dungeon Data")]
    [SerializeField] FloorSettings floorSettings;
    public List<GameObject> enemiesAlive;
    public int currentEnemyTurn = 0; // the index of the enemy that is currently taking its turn; 

    [Header("Player Stuff")]
    [SerializeField] GameObject playerObjectPrefab;
    [HideInInspector] public GameObject playerObject;
    public PlayerStats playerStats;
    [HideInInspector] public Tile playerTile;
    [SerializeField] GameObject cam;

    [Header("Misc")]
    public TMP_Text tileInfoText;
    [SerializeField] float enemyActionDelay = 0.25f;

    void Start(){
        pathfinding = new Pathfinding();
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
            if(Input.GetKeyDown(KeyCode.D)){
                PlayerMove(1,0);
            }
            else if(Input.GetKeyDown(KeyCode.A)){
                PlayerMove(-1,0);
            }
            else if(Input.GetKeyDown(KeyCode.W)){
                PlayerMove(0,1);
            }
            else if(Input.GetKeyDown(KeyCode.S)){
                PlayerMove(0,-1);
            }
            else if(Input.GetKeyDown(KeyCode.Z)){
                Debug.Log("Player waited!");
                PlayerTookAction();
            }
        }
    }

    private void PlayerTookAction(){  // this is called after the player acts
        controlState = ControlState.Enemy;
        if(enemiesAlive.Count > 0){
            currentEnemyTurn = 0;
            StartCoroutine(EnemyTakeTurn(enemyActionDelay));
        }
        else{
            EnemiesFinishedTurn();
        }
    }

    private void EnemiesFinishedTurn(){
        Debug.Log("Enemy turns finished, starting player turn");
        controlState = ControlState.Player;
    }

    public void PlayerMove(int right, int up){
        Tile t = gridManager.GetTileAtPosition(playerTile.x+right,playerTile.y+up);
        if(t == null){
            Debug.Log("No tile to move to!");
        }
        else if (t.currentEntity != null){
            Debug.Log("An entity is already standing at "+playerTile.x+right+", "+playerTile.y+right+"!");

            if(t.currentEntity.GetComponent<EnemyEntity>() != null){ // perform basic attack if walking into enemy
                PlayerPerformAttack(t.currentEntity.GetComponent<EnemyEntity>());
            }
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

    public void PlayerPerformAttack(EnemyEntity enem){
        Debug.Log("Player attacking "+enem.enemyName+"!");
        enem.currentHP -= 3; // Damage equation goes here, should be edited once we actually have equipment and stuff
        if(enem.currentHP <= 0){
            enemiesAlive.Remove(enem.gameObject);
            enem.enemyTile.currentEntity = null;
            Destroy(enem.gameObject);
        }
        PlayerTookAction();
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

        if(enemiesAlive.Count <= 0){
            EnemiesFinishedTurn();
        }

        GameObject obj = enemiesAlive[currentEnemyTurn];

        // Check attack range and attack if valid here (not implemented yet)
        if(false){
            ////////////////////////////////////////////////////////////////////
            //Debug.Log("HOW");
        }
        // Check for valid walkable tiles if can't attack
        else{
            Tile originalTile = obj.GetComponent<EnemyEntity>().enemyTile;

            List<Tile> enemyPathfinding = pathfinding.Astar_Pathfind(originalTile.x,originalTile.y,playerTile.x,playerTile.y,gridManager,PathfindingOption.EnemyMove);
            pathfinding.PrintTileList("Path",enemyPathfinding);

            if(enemyPathfinding != null){
                Tile t = enemyPathfinding[1]; // chooses the second item on pathfinding (first is the start tile) 
                Debug.Log("Tile: "+t.x+","+t.y);
                obj.GetComponent<EnemyEntity>().enemyTile.currentEntity = null; // removes enemy from the original tile
                t.currentEntity = obj;  // sets the enemy to the new tile
                t.MoveEntityToTile();  // physically moves the gameobject over the new tile
                obj.GetComponent<EnemyEntity>().enemyTile = t;
                Debug.Log("Enemy moved (walking towards player in vision)!");
            }
            else{ // This means the enemy has no valid route towards the player
                List<Tile> movementOptions = new List<Tile>();
                //Debug.Log(movementOptions.Count);
                
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
                    Debug.Log("Enemy moved randomly (no valid path to player)!");
                }
                else{
                    Debug.Log("Enemy couldn't move!");
                }
            }



            
        }
        
        currentEnemyTurn++;
        if(currentEnemyTurn >= enemiesAlive.Count){
            Debug.Log("All enemies have acted!");
            EnemiesFinishedTurn();
        }
        else{
            StartCoroutine(EnemyTakeTurn(enemyActionDelay));
        }
    }

    


}

public enum ControlState{
    Player, Enemy
}
