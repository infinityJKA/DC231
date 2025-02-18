using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile : MonoBehaviour
{
   [SerializeField] private Color testColor1, testColor2;
   [SerializeField] private MeshRenderer meshRenderer;
   [SerializeField] private GameObject highlight;
   public GameObject currentEntity; // This is what is currently standing on this tile.
   public int x,y;
   

    public void Init(bool isOffset){
        meshRenderer.material.color = isOffset ? testColor1 : testColor2; // sets offset colors for testing
    }

    void OnMouseEnter(){
        highlight.SetActive(true);
    }

    void OnMouseExit(){
        highlight.SetActive(false);
    }

    public void MoveEntityToTile(){
        currentEntity.transform.position = new Vector3(transform.position.x,currentEntity.transform.position.y,transform.position.z);
    }

}
