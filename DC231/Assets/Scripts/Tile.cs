using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Tile : MonoBehaviour
{
   //[SerializeField] private Color testColor1, testColor2;
   [SerializeField] public SpriteRenderer spriteRenderer;
   [SerializeField] private GameObject highlight;
   public GameObject currentEntity; // This is what is currently standing on this tile.
   public int x,y;
   public bool isHoveredOver = false;
   public bool isWalkable = true; // if the tile is walkable or not
   [HideInInspector] public GameManager gm;

   [Header("A* Pathfinding")]
   public int gCost,hCost,fCost;
   public Tile pathfindingCameFrom;

    public void Init(){

    }
    
    public void Init(bool isOffset){
        spriteRenderer.color = isOffset ? new Color(0f,0f,0f,1f) : new Color(0.2f,0.2f,0.2f,1f); // sets offset colors for testing
    }

    void OnMouseEnter(){
        highlight.SetActive(true);
        gm.currentTileHighlight = this;
        isHoveredOver = true;
        ShowTileStats();
    }

    public void ShowTileStats(){
        if(currentEntity != null){
            if(currentEntity.GetComponent<EnemyEntity>() == true){
                EnemyEntity e = currentEntity.GetComponent<EnemyEntity>();
                gm.playerStats.tileInfoText.text = e.enemyName+"\n"+e.currentHP+"/"+e.maxHP+" HP\n"+e.currentMP+"/"+e.maxMP+" MP\n"+e.def+" DEF  "+e.res+" RES"+"\n("+x+","+y+")";
            }
            else{
                gm.playerStats.tileInfoText.text = "Wall\n("+x+","+y+")";
            }
        }
        else{
            gm.playerStats.tileInfoText.text = "Empty tile\n("+x+","+y+")";
        }
    }

    void OnMouseExit(){
        highlight.SetActive(false);
        isHoveredOver = false;
    }

    public void MoveEntityToTile(){
        currentEntity.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
    }

    public void CalculateFCost(){
        fCost = gCost + hCost;
    }

    

}
