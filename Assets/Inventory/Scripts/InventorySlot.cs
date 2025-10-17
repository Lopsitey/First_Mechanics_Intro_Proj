using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        if (!ItemInitialized()) return 0;

        int newQuantity = m_ItemQuantity + quantity;

        if (newQuantity >= m_Item.MaxStack)
        {
            m_ItemQuantity = m_Item.MaxStack;
            m_QuantityText.text = m_ItemQuantity.ToString();

            return newQuantity - m_Item.MaxStack;
        }

        m_ItemQuantity = newQuantity;
        m_QuantityText.text = newQuantity.ToString();
        return 0;
    }

    public int RemoveItems(int quantity)
    {
        if (!ItemInitialized()) return 0;

        int newQuantity = m_ItemQuantity - quantity;

        if (newQuantity <= 0)
        {
            ClearItemSlot();

            m_ItemQuantity = 0;

            return newQuantity < 0 ? -newQuantity : 0;
        }

        m_ItemQuantity = newQuantity;
        m_QuantityText.text = newQuantity.ToString();
        return -1;
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
