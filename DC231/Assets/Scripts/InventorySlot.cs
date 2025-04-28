using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler//, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public InventoryManager inventoryManager;
    //[SerializeField] BoxCollider2D collision;

    private void Awake()
    {
        //Deselect();
    }

    // void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
    //     PlayerStats.instance.hoveringOverInventory = false;
    // }

    // void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
    //     PlayerStats.instance.hoveringOverInventory = true;
    //     if(transform.childCount == 0){
    //         PlayerStats.instance.tileInfoText.text = "Empty inventory slot";
    //     }
    //     else{
    //         // need to try/catch bc trash can
    //         try{
    //             Item i = transform.GetComponentInChildren<Item>();
    //             if(i.type == ItemType.Weapon){
    //                 PlayerStats.instance.tileInfoText.text = i.itemName+"\n+"+i.atkModif+" ATK\n"+i.range[0]+"-"+i.range[1]+" range";
    //             }
    //             else{
    //                 if(i.hpIncreaseAmount > 0){
    //                     PlayerStats.instance.tileInfoText.text = i.itemName+"\nGain"+i.hpIncreaseAmount+" Max HP";
    //                 }
    //                 else{
    //                     PlayerStats.instance.tileInfoText.text = i.itemName+"\nHeal"+i.healAmount+" HP";
    //                 }
    //             }
    //         }
    //         catch{return;}
    //     }
    // }

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