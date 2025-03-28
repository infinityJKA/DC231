using UnityEngine;
using UnityEngine.UI;

public class PauseButtonScript : MonoBehaviour
{
    private GameObject pauseBackground;
    private GameObject inventorySystem;
    private GameObject optionsSideBar;

    void Start()
    {
        // Find the game objects in the Start method
        pauseBackground = GameObject.Find("PauseBackground");
        inventorySystem = GameObject.Find("InventorySystem");
        optionsSideBar = GameObject.Find("OptionsBar");

        // Debug: Check if the objects are found
        if (pauseBackground == null)
        {
            Debug.LogError("PauseBackground GameObject not found!");
        }
        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem GameObject not found!");
        }
        if (optionsSideBar == null)
        {
            Debug.LogError("OptionsSideBar GameObject not found!");
        }

        // Choose whether each is active or inactive at start
        if (pauseBackground != null)
        {
            pauseBackground.SetActive(false);
        }

        if (inventorySystem != null)
        {
            inventorySystem.SetActive(false);
        }

        if (optionsSideBar != null)
        {
            optionsSideBar.SetActive(false);
        }

        // Add listener for button click (if this script is attached to the button)
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PerformClickAction);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject!");
        }
    }

    // This method will be called when the button is clicked
    public void PerformClickAction()
    {
        // Toggle visibility of pauseBackground
        if (pauseBackground != null)
        {
            bool isActive = pauseBackground.activeSelf;
            pauseBackground.SetActive(!isActive);  // Toggle the active state
        }
        else
        {
            Debug.LogError("PauseBackground GameObject not found!");
        }

        // Toggle visibility of inventorySystem
        if (inventorySystem != null)
        {
            bool isActive = inventorySystem.activeSelf;
            inventorySystem.SetActive(!isActive);  // Toggle the active state
        }
        else
        {
            Debug.LogError("InventorySystem GameObject not found!");
        }

        // Toggle visibility of optionsSideBar
        if (inventorySystem != null)
        {
            bool isActive = optionsSideBar.activeSelf;
            optionsSideBar.SetActive(!isActive);  // Toggle the active state
        }
        else
        {
            Debug.LogError("OptionsSideBar GameObject not found!");
        }
    }
}
