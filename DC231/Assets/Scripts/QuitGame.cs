using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitGameFunction()
    {
        Application.Quit();
        Debug.Log("Player Quit Game");
    }
}
