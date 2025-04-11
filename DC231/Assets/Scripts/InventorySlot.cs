using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public InventoryManager inventoryManager;

    private void Awake()
    {
        //Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    // Drag and drop
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop called.");

        if (transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
            if (inventoryItem == null)
            {
                Debug.LogError("The dragged object does not have an InventoryItem component.");
                return;
            }

            inventoryItem.parentAfterDrag = transform;
        }

        // Check if InventoryManager is assigned
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned.");
            return;
        }

        // Check if this slot is slot 9
        int slotIndex = System.Array.IndexOf(inventoryManager.inventorySlots, this);
        Debug.Log($"Slot index: {slotIndex}");

        if (slotIndex == 9)
        {
            InventoryItem inventoryItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                Debug.Log($"Deleting item: {inventoryItem.name}");
                inventoryManager.DeleteItem(inventoryItem);
            }
            else
            {
                Debug.LogError("No InventoryItem found on the dropped object.");
            }
        }
    }


}