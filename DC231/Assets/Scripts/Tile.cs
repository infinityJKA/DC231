using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Tile : MonoBehaviour
{
   [SerializeField] private Color testColor1, testColor2;
   [SerializeField] private MeshRenderer meshRenderer;
   [SerializeField] private GameObject highlight;
   public GameObject currentEntity; // This is what is currently standing on this tile.
   public int x,y;
   [HideInInspector] public GameManager gm;

   [Header("A* Pathfinding")]
   public int gCost,hCost,fCost;
   public Tile pathfindingCameFrom;

    public void Init(bool isOffset){
        meshRenderer.material.color = isOffset ? testColor1 : testColor2; // sets offset colors for testing
    }

    void OnMouseEnter(){
        highlight.SetActive(true);
        if(currentEntity != null){
            if(currentEntity.GetComponent<EnemyEntity>() == true){
                EnemyEntity e = currentEntity.GetComponent<EnemyEntity>();
                gm.tileInfoText.text = e.enemyName+"\n"+e.currentHP+"/"+e.maxHP+" HP\n"+e.currentMP+"/"+e.maxMP+" MP\n"+e.def+" DEF  "+e.res+" RES"+"\n("+x+","+y+")";
            }
        }
        else{
            gm.tileInfoText.text = "Empty tile\n("+x+","+y+")";
        }
    }

    void OnMouseExit(){
        highlight.SetActive(false);
    }

    public void MoveEntityToTile(){
        currentEntity.transform.position = new Vector3(transform.position.x,currentEntity.transform.position.y,transform.position.z);
    }

    public void CalculateFCost(){
        fCost = gCost + hCost;
    }

}
