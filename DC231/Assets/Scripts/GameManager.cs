using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GridManager gridManager;
    private Pathfinding pathfinding;
    public ControlState controlState;
    public DungeonGeneratorV2 dungeonGen;

    [Header("Dungeon Data")]
    [SerializeField] FloorSettings floorSettings;
    public List<GameObject> enemiesAlive;
    public int currentEnemyTurn = 0;
    public List<EnemyEntity> enemies1, enemies2, enemies3;
    public List<Item> items1, items2, items3;

    [Header("Player Stuff")]
    [SerializeField] GameObject playerObjectPrefab;
    [HideInInspector] public GameObject playerObject;
    public PlayerStats playerStats;
    [HideInInspector] public Tile playerTile;
    [SerializeField] GameObject cam;
    [SerializeField] Vector3 camOffset;

    [Header("Misc")]
    public Tile currentTileHighlight = null;
    [SerializeField] float enemyActionDelay = 0f;

    [Header("Game Over")]
    public GameOverScript gameOverScript;

    void Start()
    {
        pathfinding = new Pathfinding();
        enemiesAlive.Clear();

        dungeonGen.DungeonGenStart();
        gridManager.MakeGridFromRooms(dungeonGen, this);
        dungeonGen.gameObject.SetActive(false);

        playerStats = PlayerStats.instance;

        bool notSpawned = true;
        Tile tileToSpawnOn = null;
        while (notSpawned)
        {
            int i = Random.Range(0, gridManager.tiles.Count);
            if (gridManager.tiles.ElementAt(i).Value.currentEntity == null)
            {
                tileToSpawnOn = gridManager.tiles.ElementAt(i).Value;
                notSpawned = false;
            }
        }
        playerObject = Instantiate(playerObjectPrefab);
        playerTile = tileToSpawnOn;
        playerTile.currentEntity = playerObject;
        playerTile.MoveEntityToTile();
        SetCamToPlayer();
        playerStats.logText.text = "[Entered Dungeon Floor " + playerStats.dungeonFloor + "!]";
        controlState = ControlState.Player;

        // Play Theme music when game starts
        AudioManager.Instance.PlayMusic("Theme");
    }

    private void NextFloor()
    {
        controlState = ControlState.LoadingGame;
        playerStats.dungeonFloor += 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (controlState == ControlState.Player)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
                {
                    PlayerMove(1, -1);
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.A))
                {
                    PlayerMove(-1, -1);
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.A))
                {
                    PlayerMove(-1, 1);
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.D))
                {
                    PlayerMove(1, 1);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    PlayerMove(1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    PlayerMove(-1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    PlayerMove(0, 1);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    PlayerMove(0, -1);
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("Player waited!");
                    PlayerTookAction();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log("Tried to attack with mouse click!");
                    PlayerMouseAttack();
                }
            }
        }
    }

    private void PlayerTookAction()
    {
        controlState = ControlState.Enemy;
        if (enemiesAlive.Count > 0)
        {
            currentEnemyTurn = 0;
            EnemyTakeTurn();
        }
        else
        {
            EnemiesFinishedTurn();
        }
    }

    private void EnemiesFinishedTurn()
    {
        if (controlState == ControlState.GameOver)
        {
            Debug.Log("GAME OVER!");
        }
        else
        {
            Debug.Log("Enemy turns finished, starting player turn");
            controlState = ControlState.Player;
        }
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public void PlayerMouseAttack()
    {
        if (currentTileHighlight != null)
        {
            if (currentTileHighlight.currentEntity != null & currentTileHighlight.isHoveredOver)
            {
=======
=======
>>>>>>> Stashed changes
    public void UseConsumableItem(){
        Item i = GetCurrentlyEquippedItem();
        if(i.hpIncreaseAmount > 0){
            playerStats.maxHP += i.hpIncreaseAmount;
            playerStats.logText.text = playerStats.logText.text + "\n= Used "+i.itemName+"!\n<Max HP increased by "+i.hpIncreaseAmount+">";
        }
        else{
            playerStats.currentHP += i.healAmount;
            if(playerStats.currentHP > playerStats.maxHP){playerStats.currentHP = playerStats.maxHP;}

            playerStats.logText.text = playerStats.logText.text + "\n< Used "+i.itemName+" to heal HP.";
        }
        playerStats.UpdateHPText();
        playerStats.inventory.DeleteItem(playerStats.inventory.inventorySlots[playerStats.inventory.selectedGameSlot].GetComponentInChildren<InventoryItem>());
        PlayerTookAction();
    }

    public void PlayerMouseAttack(){
        bool usedConsumable = false;
        if(GetCurrentlyEquippedItem() != null){
            if(GetCurrentlyEquippedItem().type == ItemType.Consumable){
                usedConsumable = true;
                UseConsumableItem();
            }
        }
        if(currentTileHighlight != null && !usedConsumable){
            if(currentTileHighlight.currentEntity != null & currentTileHighlight.isHoveredOver){
>>>>>>> Stashed changes
                int minRange = 0;
                int maxRange = 1;
                if (GetCurrentlyEquippedItem() != null)
                {
                    maxRange = GetCurrentlyEquippedItem().range[1];
                    minRange = GetCurrentlyEquippedItem().range[0];
                }

                List<Tile> tiles = pathfinding.Astar_Pathfind(playerTile.x, playerTile.y, currentTileHighlight.x, currentTileHighlight.y, gridManager, PathfindingOption.AttackRange, maxRange);
                if (tiles == null)
                {
                    Debug.Log("Too far to attack");
                    return;
                }
                int dist = tiles.Count - 1;
                Debug.Log("PlayerMouseAttackDist = " + dist);
                if (currentTileHighlight.currentEntity.GetComponent<EnemyEntity>())
                {
                    if (GetCurrentlyEquippedItem() == null)
                    {
                        if (dist == 1)
                        {
                            PlayerPerformAttack(currentTileHighlight.currentEntity.GetComponent<EnemyEntity>());
                        }
                        Debug.Log("PlayerMouseAttack enemy is not in attack range (nothing equipped)");
                    }
                    else if (minRange <= dist & maxRange >= dist)
                    {
                        PlayerPerformAttack(currentTileHighlight.currentEntity.GetComponent<EnemyEntity>());
                    }
                    else
                    {
                        Debug.Log("PlayerMouseAttack enemy is not in attack range");
                    }
                }
                else
                {
                    Debug.Log("PlayerMouseAttack target is not an enemy");
                }
            }
        }
    }

    public void PlayerMove(int right, int up)
    {
        Tile t = gridManager.GetTileAtPosition(playerTile.x + right, playerTile.y + up);
        if (t == null)
        {
            Debug.Log("No tile to move to!");
        }
        else if (t.currentEntity != null)
        {
            Debug.Log("An entity is already standing at " + playerTile.x + right + ", " + playerTile.y + up + "!");

            if (t.currentEntity.GetComponent<EnemyEntity>() != null)
            {
                PlayerPerformAttack(t.currentEntity.GetComponent<EnemyEntity>());
            }
            else if (t.currentEntity.GetComponent<floorExit>() != null)
            {
                NextFloor();
                return;
            }
            else if (t.currentEntity.GetComponent<Chest>() != null)
            {
                Debug.Log("Item = " + t.currentEntity.GetComponent<Chest>().item.itemName);
                bool added = playerStats.inventory.AddItem(t.currentEntity.GetComponent<Chest>().item);
                if (added)
                {
                    playerStats.logText.text = playerStats.logText.text + "\nFound " + t.currentEntity.GetComponent<Chest>().item.itemName + "!";
                    Destroy(t.currentEntity.gameObject);
                    AudioManager.Instance.PlaySFX("ChestOpen");
                    t.currentEntity = null;

                    t.currentEntity = playerObject;
                    playerTile.currentEntity = null;
                    playerTile = t;
                    t.MoveEntityToTile();
                    SetCamToPlayer();
                    PlayerTookAction();
                }
                else
                {
                    playerStats.logText.text = playerStats.logText.text + "\nINVENTORY IS FULL";
                }
            }
        }
        else
        {
            Debug.Log("Moving from " + playerTile.x + "," + playerTile.y + " to " + t.x + "," + t.y);
            t.currentEntity = playerObject;
            playerTile.currentEntity = null;
            playerTile = t;
            t.MoveEntityToTile();
            AudioManager.Instance.PlaySFX("FootStep");
            SetCamToPlayer();
            PlayerTookAction();
        }
    }

    public Item GetCurrentlyEquippedItem()
    {
        if (playerStats.inventory.selectedGameSlot < 0)
        {
            Debug.Log("Nothing equipped");
            return null;
        }
        if(playerStats.inventory.inventorySlots[playerStats.inventory.selectedGameSlot].GetComponentInChildren<InventoryItem>() == null){
            return null;
        }
        return playerStats.inventory.inventorySlots[playerStats.inventory.selectedGameSlot].GetComponentInChildren<InventoryItem>().item;
    }

    public void PlayerPerformAttack(EnemyEntity enem)
    {
        Debug.Log("Player attacking " + enem.enemyName + "!");
        AudioManager.Instance.PlaySFX("Swing");

        int dmg = 1;
        Item currentItem = GetCurrentlyEquippedItem();
        if (currentItem != null)
        {
            if (currentItem.type == ItemType.Weapon)
            {
                dmg += currentItem.atkModif;
                Debug.Log(currentItem.name + " is equipped during attack!  DMG = " + dmg);
            }
        }
        enem.currentHP -= dmg;
        playerStats.logText.text = playerStats.logText.text + "\n= Attacked " + enem.enemyName + " for " + dmg + " dmg!";
        if (enem.currentHP <= 0)
        {
            enemiesAlive.Remove(enem.gameObject);
            enem.enemyTile.currentEntity = null;
            playerStats.logText.text = playerStats.logText.text + "\n= Defeated " + enem.enemyName + "!";
            Destroy(enem.gameObject);
        }
        UpdateUI();
        PlayerTookAction();
    }

    public void SetCamToPlayer()
    {
        cam.transform.position = new Vector3(playerObject.transform.position.x + camOffset.x, playerObject.transform.position.y + camOffset.y, playerObject.transform.position.z + camOffset.z);
    }

    public bool CheckIfTileIsValidWalk(int right, int up, Tile tile)
    {
        Tile t = gridManager.GetTileAtPosition(tile.x + right, tile.y + up);
        if (t == null)
        {
            Debug.Log("No tile to move to!");
        }
        else if (t.currentEntity != null)
        {
            Debug.Log("An entity is already standing at " + playerTile.x + right + ", " + playerTile.y + right + "!");
        }
        else if (right == playerTile.x && up == playerTile.y)
        {
            Debug.Log("The player is standing here! Invalid move location!");
        }
        else
        {
            return true;
        }
        return false;
    }

    public Tile TileAddCord(int x, int y, Tile tile)
    {
        return gridManager.GetTileAtPosition(tile.x + x, tile.y + y);
    }

    public void CheckIfPlayerAlive()
    {
        if (playerStats.currentHP <= 0)
        {
            controlState = ControlState.GameOver;
            Destroy(playerObject.gameObject);
            Debug.Log("Player has died!");
            playerStats.logText.text = playerStats.logText.text + "\n  [GAME OVER] You have died.";

            AudioManager.Instance.FadeOutMusicAndPlay("GameOver"); // ✅ Correct

            if (gameOverScript != null)
            {
                gameOverScript.ActivateGameOverScreen();
            }
            else
            {
                Debug.LogError("GameOverScript is not assigned in the GameManager.");
            }
        }
    }

    public void EnemyPerformAttack(Tile enemyTile)
    {
        EnemyEntity enem = enemyTile.currentEntity.GetComponent<EnemyEntity>();
        int dmg = enem.atk;
        playerStats.currentHP -= dmg;
        playerStats.logText.text = playerStats.logText.text + "\n* " + enem.enemyName + " attacked you for " + dmg + " dmg!";
        Debug.Log("Player got attacked by an enemy!");
        AudioManager.Instance.PlaySFX("EnemyAttack");
        UpdateUI();
        CheckIfPlayerAlive();
    }

    private void EnemyTakeTurn()
    {
        if (enemiesAlive.Count <= 0 || controlState == ControlState.GameOver)
        {
            EnemiesFinishedTurn();
        }

        GameObject obj = enemiesAlive[currentEnemyTurn];
        Tile originalTile = obj.GetComponent<EnemyEntity>().enemyTile;

        List<Tile> attackDistance = pathfinding.Astar_Pathfind(originalTile.x, originalTile.y, playerTile.x, playerTile.y, gridManager, PathfindingOption.AttackRange, obj.GetComponent<EnemyEntity>().attackRange);

        if (attackDistance != null && attackDistance.Count - 1 >= obj.GetComponent<EnemyEntity>().minAttackRange && obj.GetComponent<EnemyEntity>().attackRange >= attackDistance.Count - 1)
        {
            EnemyPerformAttack(originalTile);
        }
        else
        {
            List<Tile> enemyPathfinding = pathfinding.Astar_Pathfind(originalTile.x, originalTile.y, playerTile.x, playerTile.y, gridManager, PathfindingOption.EnemyMove, obj.GetComponent<EnemyEntity>().vision);

            if (enemyPathfinding != null && enemyPathfinding[1] != playerTile)
            {
                Tile t = enemyPathfinding[1];
                obj.GetComponent<EnemyEntity>().enemyTile.currentEntity = null;
                t.currentEntity = obj;
                t.MoveEntityToTile();
                obj.GetComponent<EnemyEntity>().enemyTile = t;
            }
        }

        currentEnemyTurn++;
        if (currentEnemyTurn >= enemiesAlive.Count)
        {
            EnemiesFinishedTurn();
        }
        else
        {
            EnemyTakeTurn();
        }
    }

    public void UpdateUI()
    {
        playerStats.hpText.text = "HP: " + playerStats.currentHP + "/" + playerStats.maxHP;
        if (currentTileHighlight != null)
        {
            currentTileHighlight.ShowTileStats();
        }
    }
}

public enum ControlState
{
    Player, Enemy, GameOver, LoadingGame
}
