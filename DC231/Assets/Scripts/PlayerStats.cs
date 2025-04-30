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
    public TMP_Text tileInfoText,logText,hpText,scoreText;
    public static PlayerStats instance;
    public GameObject gameplayUI;
    public GameObject gameOverMenu;
    public bool hoveringOverInventory = false;

    public int score;

    public BiomeInfo[] biomes;
    public int biomeIndex;

    public void ResetPlayerStats(){
        currentHP = 20;
        maxHP = 20;
        dungeonFloor = 1;
        score = 0;
        inventory.ResetInventory();
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

    public void UpdateHPText(){
        hpText.text = "HP: "+currentHP+"/"+maxHP;
    }

    public void AddScore(int s){
        score += s;
        UpdateScoreText();
    }

    public void UpdateScoreText(){
        scoreText.text = "Score: "+score;
    }

}
