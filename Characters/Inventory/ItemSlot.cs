//Copyright (c) 2024 Names Are Hard, All Rights Reserved
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    //====== ITEM DATA ====== //
    public string itemName; 
    public int quantity;
    public Sprite itemImage;
    public bool isFull;

    //====== ITEM SLOT ====== //
    [SerializeField] 
    private TMP_Text quantityText;
    [SerializeField]
    private Image itemImageComponent;

    public void AddItem(string itemName, int quantity, Sprite image)
    {
        Debug.Log("Added " + quantity + " " + itemName);
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemImage = image;
        isFull = false;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImageComponent.sprite = image;
    }
}