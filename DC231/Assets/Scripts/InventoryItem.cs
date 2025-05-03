using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    void Start()
    {
        InitialiseItem(item);
        if (image == null)
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("Image component is not set and could not be found on the GameObject.");
            }
        }
    }

    

    [Header("UI")]
    public Image image;
    public Text countText;

    public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
        PlayerStats.instance.hoveringOverInventory = false;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
        PlayerStats.instance.hoveringOverInventory = true;
        if(item.type == ItemType.Weapon){
            PlayerStats.instance.tileInfoText.text = item.itemName+"\n+"+item.atkModif+" ATK\n"+item.range[0]+"-"+item.range[1]+" range";
        }
        else if(item.type == ItemType.Consumable){
            if(item.hpIncreaseAmount > 0){
                PlayerStats.instance.tileInfoText.text = item.itemName+"\nGain "+item.hpIncreaseAmount+" Max HP";
            }
            else{
                PlayerStats.instance.tileInfoText.text = item.itemName+"\nHeal "+item.healAmount+" HP";
            }
        }
        else{
            PlayerStats.instance.tileInfoText.text = item.itemName+"\nCan be epically placed and picked up.";
        }
            
    }

    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        //RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
    }

    // Drag and drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null)
        {
            Debug.LogError("Image component is not set.");
            return;
        }
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (image == null)
        {
            Debug.LogError("Image component is not set.");
            return;
        }
        if (parentAfterDrag == null)
        {
            Debug.LogError("parentAfterDrag is not set.");
            return;
        }
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
