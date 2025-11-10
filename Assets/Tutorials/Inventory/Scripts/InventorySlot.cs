using System;
using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int ItemID = -1;
    private int m_ItemQuantity = 0;

    private ItemSO m_Item;

    private TextMeshProUGUI m_ItemName;
    private TextMeshProUGUI m_QuantityText;
    private UnityEngine.UI.Image m_Icon;

    void Start()
    {
        name = "Empty Slot";

        Transform iconHolder = transform.Find("Item Background");

        m_ItemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        m_QuantityText = transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
        m_Icon = iconHolder.Find("Icon").GetComponent<UnityEngine.UI.Image>();

        ClearItemSlot();

    }

    /// <summary>
    /// Item slots must be initialized/filled before use.
    /// </summary>
    /// <param name="item">Item to be initialised</param>
    /// <param name="quantity">The amount of the item to be initialised</param>
    public void InitItemSlot(ItemSO item, int quantity)
    {
        int excess = Math.Max(-item.MaxStack + quantity, 0);

        name = item.ItemName + " Slot";
        m_ItemName.text = item.ItemName;
        m_Icon.sprite = item.ItemIcon;
        m_Item = item;
        m_ItemQuantity = excess > 0 ? item.MaxStack : quantity;
        m_QuantityText.text = m_ItemQuantity.ToString();

        ItemID = item.ItemID;

        if (excess > 0) Debug.LogWarning("Added stack contained more items than can fit into a single stack");
    }

    public ItemSO GetAttachedItem()
    {
        if (!ItemInitialized()) return null;

        return m_Item;
    }

    public void SetItemQuantity(int quantity)
    {
        if (!ItemInitialized()) return;

        m_QuantityText.text = quantity.ToString();
        m_ItemQuantity = quantity;

        if (quantity <= 0)
        {
            ClearItemSlot();
        }
    }

    public int AddItems(int quantity)
    {
        if (!ItemInitialized()) return 0;//validity check

        int newQuantity = m_ItemQuantity + quantity;//updates quantity

        if (newQuantity >= m_Item.MaxStack)//if the stack should be full
        {
            m_ItemQuantity = m_Item.MaxStack;//fill the stack
            m_QuantityText.text = m_ItemQuantity.ToString();//updates the UI

            return newQuantity - m_Item.MaxStack;//returns the amount of items which couldn't be added to the current stack 
        }

        m_ItemQuantity = newQuantity;//updates the quantity
        m_QuantityText.text = newQuantity.ToString();//updates the UI
        return 0;//no overflow so returns zero
    }

    public int RemoveItems(int quantity)
    {
        if (!ItemInitialized()) return 0;//another validity check

        int newQuantity = m_ItemQuantity - quantity;//performs the item removal calc

        if (newQuantity <= 0)//if enough of the item has been removed
        {
            ClearItemSlot();//clear the slot

            m_ItemQuantity = 0;//clear the quantity on that item
            
            return newQuantity < 0 ? -newQuantity : 0;//-newQuantity would return positive if newQuantity is negative 
        }

        m_ItemQuantity = newQuantity;//sets the item quantity if it is positive 
        m_QuantityText.text = newQuantity.ToString();//updates the UI to reflect the new quantity
        return -1;//returns -1 to signify the quantity was updated to a positive successfully  
    }

    public void ClearItemSlot()
    {
        name = "Empty Slot";
        m_Item = null;
        ItemID = -1;
        m_ItemName.text = "";
        m_QuantityText.text = "";
        m_ItemQuantity = 0;
        m_Icon.sprite = null;
    }

    private bool ItemInitialized()
    {
        if (ItemID == -1 || m_Item == null)
        {
            Debug.LogError("Item Slot has not been properly initialized");

            return false;
        }

        return true;
    }
}
