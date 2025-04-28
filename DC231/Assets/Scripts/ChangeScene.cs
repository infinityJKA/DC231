using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string TargetScene;

    public void LoadScene()
    {
        SceneManager.LoadScene(TargetScene);
    }

    public void GameStartStats(){
        if(PlayerStats.instance != null){
            PlayerStats.instance.gameplayUI.SetActive(true);
            PlayerStats.instance.ResetPlayerStats();
            PlayerStats.instance.UpdateHPText();
        }
    }

    public void HideGameplayUI(){
        PlayerStats.instance.gameplayUI.SetActive(false);
    }
}
