using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject parentCanvas;
    // // Activates this GameObject and all its children
    // public void ActivateGameOverScreen()
    // {
    //     Debug.Log("ActivateGameOverScreen called.");

    //     //Disable gameplay UI
    //     PlayerStats.instance.gameplayUI.SetActive(false);       

    //     // Activate this GameObject
    //     gameObject.SetActive(true);

    //     // Activate all child GameObjects
    //     foreach (Transform child in transform)
    //     {
    //         child.gameObject.SetActive(true);
    //     }
    // }

    // // Deactivates this GameObject and all its children
    // public void DeactivateGameOverScreen()
    // {
    //     Debug.Log("DeactivateGameOverScreen called.");

    //     // Deactivate this GameObject
    //     gameObject.SetActive(false);

    //     // Deactivate all child GameObjects
    //     foreach (Transform child in transform)
    //     {
    //         child.gameObject.SetActive(false);
    //     }
    // }

    public void RestartGame()
    {
        Debug.Log("RestartGame called.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        parentCanvas.SetActive(false);
    }
}
