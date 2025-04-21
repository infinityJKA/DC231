using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int currentHP,maxHP;
    public int dungeonFloor = 1;
    public InventoryManager inventory;
    public TMP_Text tileInfoText,logText,hpText;
    public static PlayerStats instance;

    public void ResetPlayerStats(){
        currentHP = 20;
        maxHP = 20;
        dungeonFloor = 1;
    }

    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
