using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreHomeMenu : MonoBehaviour
{
    [SerializeField] TMP_Text highscoreText;
    void Start()
    {
        if(PlayerPrefs.HasKey("HighScore")){
            highscoreText.text = "High Score:\n"+PlayerPrefs.GetInt("HighScore");
        }
        else{
            highscoreText.text = "High Score:\0";
        }  
    }
}
